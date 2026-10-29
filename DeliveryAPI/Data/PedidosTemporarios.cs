using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Guarda pedidos em memória até a integração com o MySQL.
/// Os pedidos são perdidos quando a aplicação é reiniciada.
/// </summary>
public static class PedidosTemporarios
{
    public static List<Pedido> Pedidos { get; } = new();

    private static int proximoId = 1;

    public static int GerarProximoId()
    {
        return proximoId++;
    }
}
