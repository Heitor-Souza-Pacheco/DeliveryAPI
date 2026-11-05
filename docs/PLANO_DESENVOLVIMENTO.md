# Plano de desenvolvimento — DeliveryAPI

## Objetivo

Concluir a lógica principal da API de delivery de rosquinhas antes de criar o banco de dados em sala de aula. Até lá, os dados serão armazenados temporariamente em memória para permitir o desenvolvimento e os testes pelo Postman.

> **Importante:** o armazenamento em memória é provisório. Os dados serão perdidos quando a aplicação reiniciar. A persistência com MySQL será implementada depois.

## Estratégia de desenvolvimento

1. Revisar e organizar a estrutura existente do projeto.
2. Definir os modelos e as regras de validação.
3. Finalizar cadastro e login com seleção do tipo de conta.
4. Implementar o catálogo fixo de produtos.
5. Implementar o carrinho e os cálculos de preço.
6. Implementar a finalização dos pedidos e o histórico.
7. Permitir que entregadores consultem todos os pedidos e atualizem seus status.
8. Testar os fluxos e os erros no Postman.
9. Em sala de aula, criar o banco MySQL e integrar a API usando Entity Framework Core, conforme permitido pelo professor.

## Estrutura do projeto

Manter a arquitetura simples e compatível com o projeto atual:

- `Model/`: classes que representam os dados da aplicação.
- `Controller/`: endpoints HTTP e respostas da API.
- `Data/`: armazenamento temporário e futura configuração de acesso ao banco.
- `Service/`: adicionar apenas quando uma regra de negócio justificar separar essa responsabilidade do controller.

A estrutura poderá ser ajustada conforme as necessidades reais, evitando criar camadas sem necessidade.

## Funcionalidades e endpoints

### 1. Usuários e identificação da conta

- `POST /api/usuario`: cadastrar usuário como `Cliente` ou `Entregador`.
- `POST /api/usuario/login`: receber e-mail, senha e tipo de conta selecionado na tela.
- Conferir se as credenciais e o tipo selecionado correspondem ao usuário cadastrado.
- Retornar os dados públicos do usuário, incluindo ID e tipo de conta, sem retornar a senha.
- Validar campos obrigatórios e e-mail duplicado.

A identificação é simplificada para o projeto escolar. Não haverá JWT ou hash de senha nesta etapa. Os IDs enviados nas rotas não substituem autenticação e autorização reais.

### 2. Catálogo de produtos

- `GET /api/produto`: listar as rosquinhas disponíveis.
- `GET /api/produto/{id}`: consultar um produto.
- O catálogo será definido pela aplicação. Clientes não poderão cadastrar, alterar ou excluir produtos.
- Preços e disponibilidade serão controlados pelo backend.

### 3. Carrinho

- `GET /api/carrinho/{usuarioId}`: consultar o carrinho do cliente.
- `POST /api/carrinho/{usuarioId}/itens`: adicionar um produto ou aumentar sua quantidade.
- `PUT /api/carrinho/{usuarioId}/itens/{produtoId}`: alterar a quantidade.
- `DELETE /api/carrinho/{usuarioId}/itens/{produtoId}`: remover um item.
- Validar produto existente, quantidade válida e limites definidos.
- O total será calculado no backend usando os preços oficiais, nunca valores enviados pelo frontend.

### 4. Pedidos

- `POST /api/pedido/{clienteId}`: finalizar a compra e criar um pedido.
- `GET /api/pedido/cliente/{clienteId}`: listar os pedidos de um cliente.
- `GET /api/pedido/{pedidoId}`: consultar um pedido por ID.
- Registrar os itens, quantidades e preços praticados no momento da compra.
- Validar carrinho não vazio antes da finalização.

### 5. Entregas

- `GET /api/pedido/entregador/{entregadorId}`: listar todos os pedidos para um entregador cadastrado.
- `PUT /api/pedido/{pedidoId}/status`: atualizar o status de um pedido.
- Todos os entregadores cadastrados podem visualizar todos os pedidos.
- Qualquer entregador cadastrado pode atualizar o status de qualquer pedido.
- Não haverá atribuição individual nem priorização de pedidos.
- O fluxo de status será: `Recebido` -> `EmPreparacao` -> `SaiuParaEntrega` -> `Entregue`.

## Armazenamento temporário antes do MySQL

Enquanto o banco não for criado, coleções em memória poderão simular usuários, produtos, carrinhos e pedidos. O objetivo é validar a lógica e os contratos HTTP.

- Centralizar o armazenamento temporário para facilitar sua substituição.
- Não espalhar listas estáticas e regras de persistência por vários controllers.
- Não considerar os dados de teste permanentes.
- Ao integrar o MySQL, transferir o acesso aos dados para o contexto/repositórios necessários e criar as relações e tabelas com o grupo.

## Modelo de dados previsto para a etapa do banco

A estrutura final será confirmada antes da criação das tabelas, mas deverá contemplar, no mínimo:

- **Usuario:** dados do cliente ou entregador e credenciais.
- **Produto:** nome, descrição opcional, preço e disponibilidade.
- **Pedido:** cliente, data, status e total.
- **ItemPedido:** produto, quantidade e preço unitário registrado na compra.
- **Relacionamentos:** um usuário pode ter vários pedidos; um pedido contém vários itens; cada item referencia um produto.

## Roteiro de testes no Postman

1. Cadastrar um usuário válido como cliente.
2. Cadastrar um usuário válido como entregador.
3. Tentar cadastrar novamente um e-mail já utilizado.
4. Fazer login com credenciais e tipo de conta corretos.
5. Tentar fazer login selecionando um tipo diferente do cadastro.
6. Consultar o catálogo e um produto inexistente.
7. Adicionar produtos ao carrinho e alterar quantidades.
8. Testar remoção e quantidades inválidas.
9. Conferir se o backend calcula o total corretamente.
10. Tentar finalizar um carrinho vazio e um carrinho válido.
11. Consultar o histórico do cliente.
12. Consultar todos os pedidos usando o ID de um entregador.
13. Tentar consultar a lista de entregador usando ID de cliente.
14. Testar as transições de status, inclusive tentativas de pular etapas.

## Critério para considerar o backend pronto antes do MySQL

A etapa pré-MySQL estará concluída quando os endpoints implementados responderem corretamente, as validações e regras de negócio estiverem testadas no Postman e a estrutura estiver preparada para substituir o armazenamento em memória pela persistência real.

A conclusão dessa etapa **não significa** que os dados já são persistentes. Essa parte dependerá da criação e integração do MySQL em sala de aula.
