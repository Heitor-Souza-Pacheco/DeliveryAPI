using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(254, ErrorMessage = "O email deve ter no máximo 254 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo de conta.")]
    [RegularExpression("^(Cliente|Entregador)$", ErrorMessage = "O tipo de conta deve ser Cliente ou Entregador.")]
    public string TipoConta { get; set; } = string.Empty;
}
