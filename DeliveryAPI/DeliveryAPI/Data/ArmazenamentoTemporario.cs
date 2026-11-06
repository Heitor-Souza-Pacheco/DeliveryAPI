using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Armazenamento temporário em memória enquanto o banco de dados não é criado.
/// Os dados são perdidos quando a aplicação é reiniciada.
/// Esta classe será substituída pela persistência com MySQL posteriormente.
/// </summary>
// static significa que usamos esta classe diretamente, sem criar um objeto dela.
public static class ArmazenamentoTemporario
{
    // A lista fica acessível pela classe e começa vazia.
    public static List<Usuario> Usuarios { get; } = new();

    private static int proximoId = 1;

    // Devolve o ID atual e depois incrementa o contador (proximoId++).
    public static int GerarProximoId()
    {
        return proximoId++;
    }
}
