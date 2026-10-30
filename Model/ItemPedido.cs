namespace DeliveryAPI.Models;

// Guarda a cópia dos dados do produto no momento em que o pedido é criado. (pode ser usado para histórico)
public class ItemPedido
{
    public int ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }
    
    public decimal Subtotal => PrecoUnitario * Quantidade;
}
