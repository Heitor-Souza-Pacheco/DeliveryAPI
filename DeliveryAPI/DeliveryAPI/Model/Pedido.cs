namespace DeliveryAPI.Models;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Recebido";
    public string? EnderecoEntrega { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(item => item.Subtotal);
}
