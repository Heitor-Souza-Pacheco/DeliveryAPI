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
    [StringLength(191, ErrorMessage = "O email deve ter no máximo 191 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo de conta.")]
    [RegularExpression("^(Cliente|Entregador)$", ErrorMessage = "O tipo de conta deve ser Cliente ou Entregador.")]
    public string TipoConta { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [StringLength(160, ErrorMessage = "O endereço deve ter no máximo 160 caracteres.")]
    public string? Endereco { get; set; }
}
