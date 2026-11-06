// Cria o construtor que configura os serviços e opções da aplicação web.
var builder = WebApplication.CreateBuilder(args);

// Registra o suporte a controllers para receber requisições HTTP.
builder.Services.AddControllers();

// Permite que o frontend local acesse a API durante o desenvolvimento.
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

// Constrói a aplicação com os serviços configurados acima.
var app = builder.Build();

// Aplica a política de CORS antes de mapear os controllers.
app.UseCors("FrontendPolicy");

// Faz com que as rotas definidas nos controllers sejam atendidas.
app.MapControllers();

// Inicia o servidor web e passa a receber requisições.
app.Run();
