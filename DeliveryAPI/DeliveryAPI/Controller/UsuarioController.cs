using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public UsuarioController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
    {
        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim();
        usuario.TipoConta = usuario.TipoConta.Trim();
        usuario.Telefone = usuario.Telefone?.Trim();
        usuario.Endereco = usuario.Endereco?.Trim();

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            return BadRequest("O nome não pode conter apenas espaços.");

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            return BadRequest("O telefone é obrigatório.");

        if (usuario.TipoConta == "Cliente" && string.IsNullOrWhiteSpace(usuario.Endereco))
            return BadRequest("O endereço é obrigatório para clientes.");

        bool emailExiste = await _context.Usuarios.AnyAsync(u =>
            u.Email.ToLower() == usuario.Email.ToLower());

        if (emailExiste)
            return Conflict("Este email já está cadastrado.");

        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(DadosPublicos(usuario));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _context.Usuarios
            .Select(usuario => new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.TipoConta
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuario requisicao)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
            return NotFound("Usuário não encontrado.");

        requisicao.Nome = requisicao.Nome.Trim();
        requisicao.Email = requisicao.Email.Trim();
        requisicao.Telefone = requisicao.Telefone.Trim();
        requisicao.Endereco = requisicao.Endereco?.Trim();

        if (string.IsNullOrWhiteSpace(requisicao.Nome))
            return BadRequest("O nome não pode conter apenas espaços.");

        if (string.IsNullOrWhiteSpace(requisicao.Telefone))
            return BadRequest("O telefone é obrigatório.");

        if (usuario.TipoConta == "Cliente" && string.IsNullOrWhiteSpace(requisicao.Endereco))
            return BadRequest("O endereço é obrigatório para clientes.");

        bool emailExiste = await _context.Usuarios.AnyAsync(u =>
            u.Id != id && u.Email.ToLower() == requisicao.Email.ToLower());

        if (emailExiste)
            return Conflict("Este email já está cadastrado.");

        usuario.Nome = requisicao.Nome;
        usuario.Email = requisicao.Email;
        usuario.Telefone = requisicao.Telefone;
        usuario.Endereco = requisicao.Endereco;

        if (!string.IsNullOrWhiteSpace(requisicao.Senha))
            usuario.Senha = _passwordHasher.HashPassword(usuario, requisicao.Senha);

        await _context.SaveChangesAsync();
        return Ok(DadosPublicos(usuario));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        string email = login.Email.Trim();
        string tipoConta = login.TipoConta.Trim();

        Usuario? usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.ToLower());

        if (usuario == null ||
            !usuario.TipoConta.Equals(tipoConta, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Email, senha ou tipo de conta incorretos.");

        bool senhaValida = false;

        if (usuario.Senha.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, login.Senha);
            senhaValida = resultado != PasswordVerificationResult.Failed;

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
                usuario.Senha = _passwordHasher.HashPassword(usuario, login.Senha);
        }
        else if (usuario.Senha == login.Senha)
        {
            // Atualiza a senha antiga em texto puro para o formato protegido no próximo login.
            usuario.Senha = _passwordHasher.HashPassword(usuario, login.Senha);
            senhaValida = true;
        }

        if (!senhaValida)
            return Unauthorized("Email, senha ou tipo de conta incorretos.");

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            usuario = DadosPublicos(usuario)
        });
    }

    private static object DadosPublicos(Usuario usuario)
    {
        return new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.TipoConta,
            usuario.Telefone,
            usuario.Endereco
        };
    }
}
