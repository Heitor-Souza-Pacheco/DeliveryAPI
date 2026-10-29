using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

// Representa um produto do catálogo de rosquinhas.
public class Produto
{
    public int Id { get; set; }

    [Required] // O nome não pode ficar vazio durante a validação do modelo.
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    // [Range] limita o valor aceito; o sufixo m indica que o número é decimal.
    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    // Inicializa como disponível quando nenhum valor é informado.
    public bool Disponivel { get; set; } = true;
}
