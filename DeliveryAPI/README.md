# 🍩 DeliveryAPI — API de Delivery de Donuts

API REST desenvolvida em C# com ASP.NET Core 9 para o projeto Donnuts & Breack. A API gerencia cadastro e login de clientes e entregadores, catálogo de produtos, carrinhos e pedidos.

## Tecnologias

- C# / .NET 9 / ASP.NET Core Web API
- Entity Framework Core
- MySQL com Pomelo
- Data Annotations para validação

## Configuração

A API utiliza MySQL. A conexão não deve ser salva no repositório.

Configure a variável de ambiente `ConnectionStrings__DefaultConnection` antes de iniciar a aplicação.

Exemplo no PowerShell (substitua os valores pelos dados do seu ambiente):

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=delivery_db;User=delivery_app;Password=<SENHA_DO_BANCO>;"
dotnet restore
dotnet run
```

A API usa a porta HTTP `5295` no perfil local. O front-end servido pelo Live Server na porta `5500` já está liberado na política CORS.

Em implantação com Nginx, configure o proxy de `/api/` para a porta local da API e sirva o front e a API no mesmo domínio.

## Banco de dados

O `AppDbContext` configura as tabelas `usuarios`, `produtos`, `itens_carrinho`, `pedidos` e `itens_pedido`.

Na inicialização, a API:

1. Garante a criação inicial do banco e das tabelas.
2. Adiciona as colunas `Telefone`, `Endereco` e `EnderecoEntrega` caso o banco já existisse antes desta integração.
3. Sincroniza os quatro produtos do catálogo com os produtos exibidos no front.

Os carrinhos e pedidos ficam persistidos no MySQL. Os quatro produtos disponíveis são Original, Chocolate, Amendoim C/ Nutella e Morango c/Confeitos.

## Endpoints

### Usuários

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/usuario` | Cadastra cliente ou entregador |
| GET | `/api/usuario` | Lista os dados básicos dos usuários |
| PUT | `/api/usuario/{id}` | Atualiza nome, e-mail, telefone, endereço e, opcionalmente, senha |
| POST | `/api/usuario/login` | Realiza login com e-mail, senha e tipo de conta |

Exemplo de cadastro:

```json
{
  "nome": "Ana Souza",
  "email": "ana@example.com",
  "telefone": "(31) 99999-9999",
  "endereco": "Rua Exemplo, 100, Centro",
  "senha": "senha123",
  "tipoConta": "Cliente"
}
```

O campo `endereco` é obrigatório para clientes. Para entregadores, o endereço não é necessário. O telefone é obrigatório para ambos.

Exemplo de login:

```json
{
  "email": "ana@example.com",
  "senha": "senha123",
  "tipoConta": "Cliente"
}
```

O tipo deve ser `Cliente` ou `Entregador` e precisa corresponder ao cadastro.

As senhas novas são armazenadas usando o `PasswordHasher` do ASP.NET Core. Contas antigas com senha em texto puro podem entrar uma vez com a senha anterior; no login bem-sucedido, a API atualiza o armazenamento para o formato protegido.

### Produtos

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/produto` | Lista produtos disponíveis |
| GET | `/api/produto/{id}` | Consulta um produto por ID |

### Carrinho

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/carrinho/{usuarioId}` | Consulta o carrinho do cliente |
| POST | `/api/carrinho/{usuarioId}/itens` | Adiciona um produto |
| PUT | `/api/carrinho/{usuarioId}/itens/{produtoId}` | Atualiza a quantidade |
| DELETE | `/api/carrinho/{usuarioId}/itens/{produtoId}` | Remove um produto |

Exemplo de inclusão:

```json
{
  "produtoId": 1,
  "quantidade": 2
}
```

### Pedidos

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/pedido/{clienteId}` | Cria pedido com os itens do carrinho |
| GET | `/api/pedido/{pedidoId}` | Consulta um pedido |
| GET | `/api/pedido/cliente/{clienteId}` | Lista pedidos do cliente |
| GET | `/api/pedido/entregador/{entregadorId}` | Lista pedidos para o entregador |
| PUT | `/api/pedido/{pedidoId}/status` | Avança o status do pedido |

A criação do pedido não recebe corpo JSON. O endereço de entrega é copiado do cadastro do cliente e o carrinho é esvaziado após a criação.

O fluxo de status é:

```text
Recebido -> EmPreparacao -> SaiuParaEntrega -> Entregue
```

Exemplo de atualização:

```json
{
  "entregadorId": 2,
  "status": "EmPreparacao"
}
```

## Integração com o front-end

Repositório: https://github.com/Heitor-Souza-Pacheco/ProjetoIntegrador-Front

- Em desenvolvimento local, o front chama `http://localhost:5295/api`.
- Em implantação com Nginx, o front chama `/api` no mesmo domínio.
- O front consome o catálogo, cadastro, login, perfil, carrinho, criação de pedido e atualização de status pelo painel de entregador.

## Observações de segurança

Este é um projeto acadêmico. O login ainda não utiliza JWT ou sessão validada no servidor, e algumas rotas identificam o usuário pelo ID enviado pelo navegador. Portanto, não utilize esta API em produção sem implementar autenticação e autorização adequadas.

Não versione senhas, tokens ou strings de conexão. Configure credenciais por variáveis de ambiente ou User Secrets.
