using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Armazena os carrinhos em memória até a integração com o MySQL.
/// Os carrinhos são perdidos quando a aplicação é reiniciada.
/// </summary>
public static class CarrinhosTemporarios
{
    // Dictionary associa cada ID de usuário à lista de itens do seu carrinho.
    // readonly impede substituir o dicionário, mas ainda permite adicionar/remover itens.
    private static readonly Dictionary<int, List<ItemCarrinho>> carrinhos = new();

    public static List<ItemCarrinho> ObterOuCriar(int usuarioId)
    {
        // Se ainda não existe carrinho para este usuário, cria uma lista vazia.
        if (!carrinhos.ContainsKey(usuarioId))
        {
            carrinhos[usuarioId] = new List<ItemCarrinho>();
        }

        return carrinhos[usuarioId];
    }
}
