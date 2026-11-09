using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
    {
        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim();
        usuario.TipoConta = usuario.TipoConta.Trim();

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            return BadRequest("O nome não pode conter apenas espaços.");

        bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == usuario.Email.ToLower());
        if (emailExiste)
            return Conflict("Este email já está cadastrado.");

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.TipoConta });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _context.Usuarios
            .Select(usuario => new { usuario.Id, usuario.Nome, usuario.Email, usuario.TipoConta })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        string email = login.Email.Trim();
        string tipoConta = login.TipoConta.Trim();

        Usuario? usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.ToLower());

        if (usuario == null ||
            usuario.Senha != login.Senha ||
            !usuario.TipoConta.Equals(tipoConta, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Email, senha ou tipo de conta incorretos.");

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            usuario = new { usuario.Id, usuario.Nome, usuario.Email, usuario.TipoConta }
        });
    }
}
