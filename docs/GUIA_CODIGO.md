# Guia simples do código — DeliveryAPI

Este documento explica os recursos de C# e ASP.NET Core usados no projeto. A ideia é entender para que servem, não decorar os nomes.

## 1. Organização dos arquivos

- **Model/**: classes que representam os dados da aplicação, como usuário, produto, carrinho e pedido.
- **Controller/**: recebe requisições HTTP, executa as regras e devolve respostas.
- **Data/**: guarda os dados temporários em memória até a integração com MySQL.
- **Program.cs**: configura e inicia o servidor.
- **DeliveryAPI.csproj**: informa ao .NET qual versão e quais dependências o projeto usa.

## 2. Anotações (Attributes) de validação

As anotações são os elementos entre colchetes colocados antes de uma propriedade ou classe. Em geral, elas descrevem regras que o framework pode verificar automaticamente.

| Anotação | Para que serve |
|---|---|
| `[Required]` | Exige que o campo seja informado. Em texto, uma string vazia também falha na validação. |
| `[StringLength(100, MinimumLength = 2)]` | Limita o tamanho do texto; neste exemplo, de 2 até 100 caracteres. |
| `[EmailAddress]` | Verifica se o texto tem formato de e-mail aceitável. Não prova que o endereço existe. |
| `[Range(1, 100)]` | Aceita apenas números dentro do intervalo, incluindo os limites. |
| `[RegularExpression("...")]` | Verifica se o texto corresponde a um padrão de expressão regular. |
| `[ApiController]` | Ativa convenções úteis para APIs, incluindo resposta automática `400 Bad Request` quando a validação do modelo falha. |
| `[Route("api/[controller]")]` | Define o caminho-base; `[controller]` é substituído pelo nome do controller sem a palavra `Controller`. |
| `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` | Indicam qual verbo HTTP e, opcionalmente, qual caminho chama o método. |
| `[FromBody]` | Diz ao ASP.NET Core para ler o parâmetro do corpo da requisição, normalmente em JSON. |

### O que significa RegularExpression?

É uma regra escrita em um padrão. Por exemplo:

```csharp
[RegularExpression("^(Cliente|Entregador)$")]
```

- `^` significa início do texto.
- `(Cliente|Entregador)` significa “Cliente” ou “Entregador”.
- `|` significa “ou”.
- `$` significa fim do texto.

Portanto, a expressão aceita somente o texto completo `Cliente` ou `Entregador`; não aceita, por exemplo, `cliente`, `Admin` ou `Cliente extra`. Expressões regulares são sensíveis a maiúsculas/minúsculas por padrão.

Na atualização de status, o padrão também limita o conjunto de status que podem ser enviados. A regra que controla a sequência correta (não pular etapas) fica no controller, separada da validação do texto.

## 3. Recursos de C# usados no projeto

### Propriedades: `get` e `set`

Exemplo: `public string Nome { get; set; }`

- `get` permite ler o valor.
- `set` permite alterar o valor.

### Inicialização com `= string.Empty` e `= new()`

Esses valores dão um estado inicial à propriedade ou coleção. `string.Empty` é um texto vazio; `new()` cria um novo objeto usando o tipo que o C# já consegue deduzir.

### `decimal` e o sufixo `m`

Preços usam `decimal`, um tipo adequado para valores monetários. O sufixo `m`, como em `3.50m`, indica que o número literal é decimal.

### `static`

Membros estáticos pertencem à classe em si, não a uma instância criada com `new`. Por isso os controllers conseguem usar `ArmazenamentoTemporario.Usuarios` diretamente. Neste projeto, isso mantém os dados em memória compartilhados enquanto a aplicação está rodando.

### `readonly`

Uma variável `readonly` não pode ser apontada para outro objeto depois da inicialização. Isso **não** torna automaticamente o conteúdo do objeto imutável: uma lista ainda pode receber itens.

### Tipos anuláveis: `Usuario?` e o valor `null`

O `?` depois de um tipo indica que a variável pode conter esse objeto ou `null`. `null` significa “nenhum objeto/nenhum resultado”. Por exemplo, `FirstOrDefault` pode retornar `null) quando não encontra correspondência.

### Operadores `?.` e `??`

- `objeto?.Nome`: acessa `Nome` somente se `objeto` não for `null`; caso contrário, o resultado é `null`.
- `valor ?? alternativa`: usa a alternativa quando o valor à esquerda é `null`.

### `=>`: lambdas e propriedades calculadas

Em `usuarios.FirstOrDefault(u => u.Id == id)`, `u => u.Id == id` é uma função curta: recebe um usuário e verifica se o ID combina.

Em `public decimal Total => Itens.Sum(...)`, a seta define uma propriedade calculada. O valor é calculado quando a propriedade é consultada, em vez de ficar guardado separadamente.

### LINQ: `Any`, `Select`, `Where`, `Sum` e outros

LINQ oferece operações para consultar coleções:

- `Any(...)`: verifica se existe pelo menos um item que atende à condição.
- `FirstOrDefault(...)`: pega o primeiro item que combina ou devolve `null` (para tipos de referência).
- `Select(...)`: transforma cada item em outro formato.
- `Where(...)`: filtra itens.
- `OrderByDescending(...)`: ordena do maior/mais recente para o menor/mais antigo.
- `Sum(...)`: soma valores.
- `ToList()`: cria uma lista com os resultados da consulta.
- `AsReadOnly()`: oferece uma visão de lista que não permite alteração por meio dessa visão.

### Objetos anônimos: `new { ... }`

Criam um objeto sem precisar declarar uma classe nova. Usamos isso nas respostas dos controllers para devolver apenas os campos públicos do usuário, sem a senha.

### `IActionResult` e respostas HTTP

Os métodos dos controllers retornam `IActionResult`, que permite escolher diferentes tipos de resposta:

- `Ok(...)`: HTTP 200, operação bem-sucedida.
- `CreatedAtAction(...)`: HTTP 201, recurso criado e com referência a uma ação que pode consultá-lo.
- `BadRequest(...)`: HTTP 400, requisição inválida.
- `Unauthorized(...)`: HTTP 401, credenciais não aceitas.
- `NotFound(...)`: HTTP 404, recurso não encontrado.
- `Conflict(...)`: HTTP 409, conflito com o estado atual, como e-mail duplicado.

### `switch` expression e `_`

A expressão `statusAtual switch { ... }` escolhe um resultado de acordo com o valor de `statusAtual`. O símbolo `_` funciona como “qualquer outro caso” e serve como padrão final.

### `nameof(...)`

Obtém o nome de um método ou membro como texto, por exemplo `nameof(BuscarPorId)`. Isso evita escrever o nome como uma string fixa e reduz erros de digitação.

### Interpolação de strings: `$"...{variavel}..."`

O `$` permite inserir valores dentro do texto usando chaves, como `$"Produto {id} indisponível"`.

### `foreach`

Percorre cada item de uma coleção e executa o bloco para cada um. No projeto, serve para copiar os itens do carrinho para o pedido.

## 4. Como funciona o início da aplicação

Em `Program.cs`:

1. `WebApplication.CreateBuilder(args)` prepara o construtor da aplicação.
2. `builder.Services.AddControllers()` registra o suporte a controllers.
3. `builder.Build()` monta a aplicação configurada.
4. `app.MapControllers()` conecta as rotas declaradas nos controllers.
5. `app.Run()` inicia o servidor e mantém a aplicação recebendo requisições.

## 5. Observações importantes do estado atual

- As validações automáticas dependem das anotações e da configuração do ASP.NET Core, incluindo `[ApiController]`.
- `Trim()` remove espaços nas extremidades, mas a validação de tamanho ocorre antes da execução do método do controller. Por isso, nomes compostos apenas por espaços precisam de um tratamento adicional após o `Trim()`.
- O cadastro e o login atuais são didáticos: a senha está armazenada e comparada em texto puro. Isso não é apropriado para produção.
- `AppDbContext.cs` está vazio e ainda não é usado; a persistência com MySQL será feita depois.
- `static` com listas e dicionários mantém os dados apenas na memória do processo. Reiniciar a API apaga esses dados.

