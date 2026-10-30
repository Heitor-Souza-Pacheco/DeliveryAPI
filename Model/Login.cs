using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// recebe somente as credenciais enviadas no login. (pode ser evoluído para Hashcode, melhorando segurança)
public class Login
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
