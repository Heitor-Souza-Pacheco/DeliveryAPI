using DeliveryAPI.Models;

namespace DeliveryAPI.Data;

/// <summary>
/// Catálogo fixo de rosquinhas para a etapa sem banco de dados.
/// Os clientes podem consultar, mas não cadastrar nem alterar produtos.
/// </summary>
public static class CatalogoProdutos
{
    private static readonly List<Produto> produtos = new()
    {
        new Produto
        {
            Id = 1,
            Nome = "Rosquinha Tradicional",
            Descricao = "Rosquinha clássica com açúcar.",
            Preco = 3.50m
        },
        new Produto
        {
            Id = 2,
            Nome = "Rosquinha de Chocolate",
            Descricao = "Rosquinha coberta com chocolate.",
            Preco = 5.00m
        },
        new Produto
        {
            Id = 3,
            Nome = "Rosquinha de Doce de Leite",
            Descricao = "Rosquinha com cobertura de doce de leite.",
            Preco = 5.50m
        },
        new Produto
        {
            Id = 4,
            Nome = "Rosquinha de Coco",
            Descricao = "Rosquinha com cobertura de coco.",
            Preco = 4.50m
        },
        new Produto
        {
            Id = 5,
            Nome = "Rosquinha de Morango",
            Descricao = "Rosquinha com cobertura sabor morango.",
            Preco = 5.00m
        }
    };

    public static IReadOnlyList<Produto> Listar()
    {
        return produtos.AsReadOnly();
    }

    public static Produto? BuscarPorId(int id)
    {
        return produtos.FirstOrDefault(produto => produto.Id == id);
    }
}
