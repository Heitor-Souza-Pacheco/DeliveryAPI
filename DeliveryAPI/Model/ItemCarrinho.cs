using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

public class ItemCarrinho
{
    public int ProdutoId { get; set; }

    [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100.")]
    public int Quantidade { get; set; }
}
