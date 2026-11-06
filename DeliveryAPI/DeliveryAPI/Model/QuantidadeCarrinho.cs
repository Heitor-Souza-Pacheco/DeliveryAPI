using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Corpo do pedido HTTP usado quando só queremos alterar a quantidade.
public class QuantidadeCarrinho
{
    [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100.")]
    public int Quantidade { get; set; }
}
