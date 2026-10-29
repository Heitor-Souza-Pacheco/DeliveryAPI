using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

public class AtualizarStatusPedido
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de entregador válido.")]
    public int EntregadorId { get; set; }

    [Required(ErrorMessage = "O status é obrigatório.")]
    [RegularExpression("^(EmPreparacao|SaiuParaEntrega|Entregue)$",
        ErrorMessage = "Status permitido: EmPreparacao, SaiuParaEntrega ou Entregue.")]
    public string Status { get; set; } = string.Empty;
}
