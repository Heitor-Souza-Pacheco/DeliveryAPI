using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
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

        bool emailExiste = ArmazenamentoTemporario.Usuarios.Any(u =>
            u.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExiste)
        {
            return Conflict("Este email já está cadastrado.");
        }

        usuario.Id = ArmazenamentoTemporario.GerarProximoId();
        ArmazenamentoTemporario.Usuarios.Add(usuario);

        // Nunca devolver a senha na resposta da API.
        return Ok(new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email
        });
    }

    [HttpGet]
    public IActionResult Listar()
    {
        // Retornar somente os dados públicos dos usuários.
        var usuarios = ArmazenamentoTemporario.Usuarios.Select(usuario => new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email
        });

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

        Usuario? usuario = ArmazenamentoTemporario.Usuarios.FirstOrDefault(u =>
            u.Email.Equals(login.Email, StringComparison.OrdinalIgnoreCase));

        if (usuario == null || usuario.Senha != login.Senha)
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
