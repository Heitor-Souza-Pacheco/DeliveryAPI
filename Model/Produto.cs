using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Representa um produto do cardapio de rosquinhas.
public class Produto
{
    public int Id { get; set; }

    [Required] // O nome não pode ficar vazio durante a validação do modelo. (tipo o not null)
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    // [Range] limita o valor aceito
    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    // Produto começa como disponível até mudança de cardápio
    public bool Disponivel { get; set; } = true;
}
