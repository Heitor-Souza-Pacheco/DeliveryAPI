// Cria o construtor que configura os serviços e opções da aplicação web.
var builder = WebApplication.CreateBuilder(args);

// Registra o suporte a controllers para receber requisições HTTP.
builder.Services.AddControllers();

// Constrói a aplicação com os serviços configurados acima.
var app = builder.Build();

// Faz com que as rotas definidas nos controllers sejam atendidas.
app.MapControllers();

// Inicia o servidor web e passa a receber requisições.
app.Run();
