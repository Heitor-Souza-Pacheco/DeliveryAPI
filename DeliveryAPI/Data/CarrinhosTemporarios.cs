using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Armazena os carrinhos em memória até a integração com o MySQL.
/// Os carrinhos são perdidos quando a aplicação é reiniciada.
/// </summary>
public static class CarrinhosTemporarios
{
    private static readonly Dictionary<int, List<ItemCarrinho>> carrinhos = new();

    public static List<ItemCarrinho> ObterOuCriar(int usuarioId)
    {
        if (!carrinhos.ContainsKey(usuarioId))
        {
            carrinhos[usuarioId] = new List<ItemCarrinho>();
        }

        return carrinhos[usuarioId];
    }
}
