using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

// [ApiController] habilita validações automáticas dos modelos.
// [Route] define a rota-base /api/usuario.
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    // [HttpPost] mapeia o método para requisições POST.
    // [FromBody] informa que os dados do usuário vêm do JSON enviado no corpo.
    public IActionResult Cadastrar([FromBody] Usuario usuario)
    {
        // Trim remove espaços no começo e no fim do texto.
        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim();
        usuario.TipoConta = usuario.TipoConta.Trim();

        // Any retorna true se ao menos um usuário satisfaz a condição.
        // Lambda (u => ...) é uma forma curta de escrever essa condição.
        // OrdinalIgnoreCase compara textos sem diferenciar maiúsculas/minúsculas.
        bool emailExiste = ArmazenamentoTemporario.Usuarios.Any(u =>
            u.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExiste)
        {
            return Conflict("Este email já está cadastrado."); // HTTP 409.
        }

        usuario.Id = ArmazenamentoTemporario.GerarProximoId();
        ArmazenamentoTemporario.Usuarios.Add(usuario);

        // Nunca devolver a senha na resposta da API.
        return Ok(new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.TipoConta
        });
    }

    [HttpGet]
    public IActionResult Listar()
    {
        // Select projeta cada usuário para um objeto com apenas os dados públicos.
        // Assim, a senha não faz parte da resposta.
        var usuarios = ArmazenamentoTemporario.Usuarios.Select(usuario => new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.TipoConta
        });

        return Ok(usuarios);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        string email = login.Email.Trim();

        // FirstOrDefault retorna o primeiro usuário correspondente ou null se não achar.
        // Usuario? indica que a variável pode conter um Usuario ou null.
        Usuario? usuario = ArmazenamentoTemporario.Usuarios.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (usuario == null || usuario.Senha != login.Senha)
        {
            return Unauthorized("Email ou senha incorretos."); // HTTP 401.
        }

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            usuario = new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.TipoConta
            }
        });
    }
}
