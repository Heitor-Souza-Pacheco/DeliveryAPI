using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Modelo simples para receber as credenciais e o tipo de conta selecionado no login.
public class Login
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo de conta.")]
    [RegularExpression("^(Cliente|Entregador)$",
        ErrorMessage = "Tipo de conta permitido: Cliente ou Entregador.")]
    public string TipoConta { get; set; } = string.Empty;
}