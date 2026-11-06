using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Modelo: representa os dados de um usuário recebidos/enviados pela API.
public class Usuario
{
    public int Id { get; set; }

    // [Required] impede campo vazio; [StringLength] define tamanho mínimo e máximo.
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    // [EmailAddress] verifica o formato básico de e-mail.
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(254, ErrorMessage = "O email deve ter no máximo 254 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    // [RegularExpression] aceita apenas os dois textos exatos indicados no padrão.
    [Required(ErrorMessage = "Selecione o tipo de conta.")]
    [RegularExpression("^(Cliente|Entregador)$", ErrorMessage = "O tipo de conta deve ser Cliente ou Entregador.")]
    public string TipoConta { get; set; } = string.Empty;
}
