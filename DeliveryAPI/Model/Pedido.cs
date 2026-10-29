namespace DeliveryAPI.Models;

// Representa um pedido realizado por um cliente.
public class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    // Valor inicial usado quando a data não é definida na criação do objeto.
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Todo pedido começa com este status.
    public string Status { get; set; } = "Recebido";

    // new() cria uma lista vazia para evitar que Itens comece como null.
    public List<ItemPedido> Itens { get; set; } = new();

    // Soma os subtotais dos itens para calcular o total do pedido.
    public decimal Total => Itens.Sum(item => item.Subtotal);
}
