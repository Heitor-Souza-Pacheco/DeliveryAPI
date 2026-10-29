using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Modelo simples para receber somente as credenciais enviadas no login.
public class Login
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
