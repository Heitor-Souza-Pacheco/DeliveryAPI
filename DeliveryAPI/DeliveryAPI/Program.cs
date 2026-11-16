using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException(
            "Configure a variável de ambiente ConnectionStrings__DefaultConnection antes de iniciar a API.");

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

Console.WriteLine("Verificando banco de dados...");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();

    // Adiciona as colunas novas em bancos que já existiam antes da integração.
    await GarantirColunaAsync(db, "usuarios", "Telefone", "VARCHAR(20) NULL");
    await GarantirColunaAsync(db, "usuarios", "Endereco", "VARCHAR(160) NULL");
    await GarantirColunaAsync(db, "pedidos", "EnderecoEntrega", "VARCHAR(160) NULL");

    // Mantém o catálogo do banco alinhado aos quatro produtos exibidos no front.
    var produtosIniciais = new[]
    {
        new Produto { Id = 1, Nome = "Original", Descricao = "Donut tradicional.", Preco = 12.00m, Disponivel = true },
        new Produto { Id = 2, Nome = "Chocolate", Descricao = "Donut com cobertura de chocolate.", Preco = 12.00m, Disponivel = true },
        new Produto { Id = 3, Nome = "Amendoim C/ Nutella", Descricao = "Donut com amendoim e Nutella.", Preco = 12.00m, Disponivel = true },
        new Produto { Id = 4, Nome = "Morango c/Confeitos", Descricao = "Donut com cobertura de morango e confeitos.", Preco = 12.00m, Disponivel = true }
    };

    foreach (var produtoInicial in produtosIniciais)
    {
        var produto = await db.Produtos.FindAsync(produtoInicial.Id);

        if (produto == null)
            db.Produtos.Add(produtoInicial);
        else
        {
            produto.Nome = produtoInicial.Nome;
            produto.Descricao = produtoInicial.Descricao;
            produto.Preco = produtoInicial.Preco;
            produto.Disponivel = true;
        }
    }

    var produtosAntigos = await db.Produtos
        .Where(p => p.Id > 4)
        .ToListAsync();

    foreach (var produto in produtosAntigos)
        produto.Disponivel = false;

    await db.SaveChangesAsync();
    Console.WriteLine("Banco de dados inicializado.");
}

app.UseCors("FrontendPolicy");
app.MapControllers();

Console.WriteLine("API iniciada.");
app.Run();

static async Task GarantirColunaAsync(
    AppDbContext db, string tabela, string coluna, string definicao)
{
    int existe = await db.Database.SqlQueryRaw<int>(
        $"SELECT COUNT(*) AS Value FROM INFORMATION_SCHEMA.COLUMNS " +
        $"WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{tabela}' AND COLUMN_NAME = '{coluna}'")
        .SingleAsync();

    if (existe == 0)
        await db.Database.ExecuteSqlRawAsync($"ALTER TABLE {tabela} ADD COLUMN {coluna} {definicao}");
}
