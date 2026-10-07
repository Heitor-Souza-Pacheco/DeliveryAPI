using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private static readonly List<Usuario> usuarios = new();
    private static int proximoId = 1;

    [HttpPost]
    public IActionResult Cadastrar([FromBody] Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
        {
            return BadRequest("O nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            return BadRequest("O email é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Senha))
        {
            return BadRequest("A senha é obrigatória.");
        }

        bool emailExiste = usuarios.Any(u =>
            u.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExiste)
        {
            return Conflict("Este email já está cadastrado.");
        }

        usuario.Id = proximoId;
        proximoId++;

        usuarios.Add(usuario);

        return Ok(usuario);
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(usuarios);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        if (string.IsNullOrWhiteSpace(login.Email))
        {
            return BadRequest("O email é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(login.Senha))
        {
            return BadRequest("A senha é obrigatória.");
        }

        Usuario? usuario = usuarios.FirstOrDefault(u =>
            u.Email.Equals(login.Email, StringComparison.OrdinalIgnoreCase));

        if (usuario == null)
        {
            return Unauthorized("Email ou senha incorretos.");
        }

        if (usuario.Senha != login.Senha)
        {
            return Unauthorized("Email ou senha incorretos.");
        }

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            usuario = new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email
            }
        });
    }
}