using DeliveryAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");

    // Evita que o Pomelo tente descobrir a versão do MySQL abrindo uma conexão
    // durante a inicialização da aplicação.
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

// Cria as tabelas e insere os produtos iniciais na primeira execução.
Console.WriteLine("Verificando banco de dados...");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("Criando/verificando as tabelas...");
    await db.Database.EnsureCreatedAsync();

    Console.WriteLine("Banco de dados inicializado.");
}

app.UseCors("FrontendPolicy");
app.MapControllers();

Console.WriteLine("API iniciada.");
app.Run();
