using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Armazenamento temporário em memória enquanto o banco de dados não é criado.
/// Os dados são perdidos quando a aplicação é reiniciada.
/// Esta classe será substituída pela persistência com MySQL posteriormente.
/// </summary>
public static class ArmazenamentoTemporario
{
    public static List<Usuario> Usuarios { get; } = new();

    private static int proximoId = 1;

    public static int GerarProximoId()
    {
        return proximoId++;
    }
}
