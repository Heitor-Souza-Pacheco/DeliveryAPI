# 🍩 DeliveryAPI — API de Delivery de Donuts

API REST desenvolvida em **C# com ASP.NET Core** para simular o funcionamento de um sistema de delivery de donuts. O projeto permite cadastrar clientes e entregadores, consultar produtos, gerenciar carrinhos, criar pedidos e acompanhar o status da entrega.

> **Estado atual:** projeto acadêmico em desenvolvimento. Os dados são armazenados temporariamente em memória; a integração com MySQL será feita posteriormente.

📘 **Está aprendendo o código?** Consulte o [guia simples de C# e ASP.NET Core](docs/GUIA_CODIGO.md), que explica as anotações, validações, LINQ, respostas HTTP e outros recursos usados no projeto.

## 🛠️ Tecnologias

- C#
- .NET 10 / ASP.NET Core Web API
- Controllers e rotas REST
- Validação de dados com Data Annotations
- Postman para testes manuais
- Armazenamento temporário em coleções estáticas (sem banco de dados nesta etapa)

## ✨ Funcionalidades

- Cadastro de usuários dos tipos `Cliente` e `Entregador`
- Login com seleção e validação do tipo de conta
- Catálogo de donuts com consulta por ID
- Carrinho por cliente, com inclusão, alteração e remoção de itens
- Cálculo de subtotal e total do carrinho
- Criação de pedidos a partir do carrinho
- Consulta de pedido por ID e listagem de pedidos por cliente
- Listagem de todos os pedidos para qualquer entregador cadastrado
- Atualização de status com validação da sequência de entrega
- Validações de entrada e respostas HTTP para situações de sucesso ou erro

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/)
- Git
- Postman (ou ferramenta equivalente para enviar requisições HTTP)

### Passos

1. Clone o repositório:

   ```bash
   git clone https://github.com/Heitor-Souza-Pacheco/DeliveryAPI.git
   cd DeliveryAPI
   ```

2. Entre na pasta do projeto:

   ```bash
   cd DeliveryAPI
   ```

3. Restaure as dependências e execute:

   ```bash
   dotnet restore
   dotnet run
   ```

4. Confira no terminal o endereço local informado pelo ASP.NET Core e use-o como base das requisições. Por exemplo, se a API estiver em `http://localhost:5295`, a URL-base será:

   ```text
   http://localhost:5295
   ```

> A porta pode variar conforme a configuração local. Use a URL exibida pelo comando `dotnet run`.

## 📚 Endpoints

Todas as rotas abaixo são relativas à URL-base da API.

### Usuários

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/usuario` | Cadastra cliente ou entregador |
| `GET` | `/api/usuario` | Lista usuários sem retornar senhas |
| `POST` | `/api/usuario/login` | Realiza login conferindo credenciais e tipo de conta |

Exemplo de cadastro (`POST /api/usuario`):

```json
{
  "nome": "Ana Souza",
  "email": "ana@example.com",
  "senha": "senha123",
  "tipoConta": "Cliente"
}
```

Para cadastrar um entregador, use `"tipoConta": "Entregador"`. O e-mail deve ser único. A resposta de cadastro inclui o ID gerado e os dados públicos do usuário, sem a senha.

Exemplo de login (`POST /api/usuario/login`):

```json
{
  "email": "ana@example.com",
  "senha": "senha123",
  "tipoConta": "Cliente"
}
```

O campo `tipoConta` deve corresponder ao tipo escolhido na tela de login e ao tipo salvo no cadastro. Se as credenciais ou o tipo não corresponderem, a API retorna `401 Unauthorized`. A resposta de sucesso inclui os dados públicos do usuário, inclusive seu ID e tipo de conta.

### Produtos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/produto` | Lista os produtos do catálogo |
| `GET` | `/api/produto/{id}` | Consulta um produto por ID |

O catálogo inicial possui cinco donuts cadastrados em memória.

### Carrinho

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/carrinho/{usuarioId}` | Consulta o carrinho do cliente |
| `POST` | `/api/carrinho/{usuarioId}/itens` | Adiciona um produto ao carrinho |
| `PUT` | `/api/carrinho/{usuarioId}/itens/{produtoId}` | Altera a quantidade de um produto |
| `DELETE` | `/api/carrinho/{usuarioId}/itens/{produtoId}` | Remove um produto do carrinho |

Exemplo de inclusão (`POST /api/carrinho/1/itens`):

```json
{
  "produtoId": 1,
  "quantidade": 2
}
```

Exemplo de alteração de quantidade (`PUT /api/carrinho/1/itens/1`):

```json
{
  "quantidade": 3
}
```

Os IDs dos exemplos são ilustrativos: use o ID real do cliente retornado pelo cadastro. A mesma inclusão de produto soma a quantidade ao item já existente, respeitando o limite definido pela API.

### Pedidos

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/pedido/{clienteId}` | Cria um pedido a partir do carrinho |
| `GET` | `/api/pedido/{pedidoId}` | Consulta um pedido por ID |
| `GET` | `/api/pedido/cliente/{clienteId}` | Lista os pedidos de um cliente |
| `GET` | `/api/pedido/entregador/{entregadorId}` | Lista todos os pedidos para um entregador cadastrado |
| `PUT` | `/api/pedido/{pedidoId}/status` | Atualiza o status do pedido |

A criação do pedido não exige corpo JSON: a API usa os itens do carrinho do cliente. Após criar o pedido, o carrinho é esvaziado. O pedido guarda uma cópia do nome e do preço unitário dos produtos no momento da criação.

Exemplo de criação:

```http
POST /api/pedido/1
```

A criação retorna `201 Created` quando concluída com sucesso.

Para o painel de entregador, use `GET /api/pedido/entregador/2`, substituindo `2` pelo ID real do entregador. Todos os entregadores cadastrados podem visualizar a mesma lista, ordenada do pedido mais recente ao mais antigo.

### Status de entrega

O fluxo permitido é:

```text
Recebido -> EmPreparacao -> SaiuParaEntrega -> Entregue
```

Exemplo de atualização (`PUT /api/pedido/1/status`):

```json
{
  "entregadorId": 2,
  "status": "EmPreparacao"
}
```

Use o ID real de um usuário cadastrado como `Entregador`. É necessário avançar na sequência permitida; pular etapas resulta em erro. Qualquer entregador cadastrado pode atualizar qualquer pedido.

## 🧪 Testando com Postman

Sugestão de ordem para testar o fluxo completo:

1. Cadastre um usuário do tipo `Cliente` e outro do tipo `Entregador`.
2. Faça login com cada usuário, enviando o respectivo `tipoConta`.
3. Tente fazer login com o tipo de conta errado e confira o retorno `401`.
4. Consulte o catálogo com `GET /api/produto`.
5. Adicione um ou mais produtos ao carrinho do cliente.
6. Consulte o carrinho e confira quantidades, subtotais e total.
7. Crie o pedido e guarde o ID retornado.
8. Consulte os pedidos do cliente com `GET /api/pedido/cliente/{clienteId}`.
9. Consulte todos os pedidos com `GET /api/pedido/entregador/{entregadorId}`.
10. Atualize o status em sequência até `Entregue`.

Entre as respostas de erro previstas estão usuário, produto ou pedido não encontrado; e-mail duplicado; carrinho vazio; produto indisponível; tipo de conta incompatível com o login; e transição de status inválida. Dados de validação incorretos podem retornar `400 Bad Request`.

## 🗂️ Organização do projeto

A estrutura principal está organizada por responsabilidade:

- `Model/`: modelos e validações dos dados, como usuário, produto, carrinho e pedido.
- `Controller/`: endpoints HTTP e regras de fluxo da API.
- `Data/`: catálogo de produtos e armazenamento temporário em memória.
- `Program.cs`: configuração e inicialização da aplicação.

## ⚠️ Limitações atuais

- **Sem MySQL por enquanto:** usuários, carrinhos e pedidos ficam em memória e são perdidos quando a aplicação é reiniciada.
- **Identificação simplificada para fins didáticos:** o login confere e-mail, senha e tipo de conta e retorna o usuário identificado, mas não implementa token de autenticação.
- **Sem autorização de produção:** os IDs enviados nas rotas identificam registros e a API confere o tipo cadastrado em operações específicas. Isso não substitui autenticação e autorização reais; os valores enviados pelo cliente podem ser alterados.
- As senhas continuam armazenadas em texto puro conforme o escopo didático atual. Isso não é apropriado para produção.

Esses pontos fazem parte do escopo atual do projeto e devem ser revistos antes de qualquer uso em produção.

## 📌 Próximas evoluções

- Integrar MySQL e persistência dos dados
- Ampliar os testes automatizados
- Melhorar documentação e tratamento de erros conforme o projeto evoluir

---

Desenvolvido como projeto de estudo para praticar construção de APIs REST com C# e ASP.NET Core.
