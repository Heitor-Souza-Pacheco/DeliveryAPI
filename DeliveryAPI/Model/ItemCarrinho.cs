using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Representa um produto e a quantidade escolhida no carrinho.
public class ItemCarrinho
{
    public int ProdutoId { get; set; }

    // [Range] permite somente quantidades de 1 a 100.
    [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100.")]
    public int Quantidade { get; set; }
}
