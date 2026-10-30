namespace DeliveryAPI.Models;

// Representa um pedido que o cliente fez.
public class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    // Valor inicial quando a data não existe
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Todo pedido começa com este status.
    public string Status { get; set; } = "Recebido";

    // new() cria uma lista vazia para evitar que tenha null no começo
    public List<ItemPedido> Itens { get; set; } = new();

    // Soma os preços dos produtos
    public decimal Total => Itens.Sum(item => item.Subtotal);
}
