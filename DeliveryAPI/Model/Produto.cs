using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

public class Produto
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    public bool Disponivel { get; set; } = true;
}
