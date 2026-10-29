using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Dados que o entregador envia ao solicitar uma mudança de status.
public class AtualizarStatusPedido
{
    // [Range] impede IDs menores que 1.
    [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de entregador válido.")]
    public int EntregadorId { get; set; }

    [Required(ErrorMessage = "O status é obrigatório.")]
    // A expressão regular libera apenas os status que podem ser escolhidos na atualização.
    [RegularExpression("^(EmPreparacao|SaiuParaEntrega|Entregue)$",
        ErrorMessage = "Status permitido: EmPreparacao, SaiuParaEntrega ou Entregue.")]
    public string Status { get; set; } = string.Empty;
}
