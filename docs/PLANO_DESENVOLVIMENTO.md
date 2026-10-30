# Plano de desenvolvimento — DeliveryAPI

## Objetivo

Concluir a lógica principal da API de delivery de rosquinhas antes de criar o banco de dados em sala de aula. Até lá, os dados serão armazenados temporariamente em memória para permitir o desenvolvimento e os testes pelo Postman.

> **Importante:** o armazenamento em memória é provisório. Os dados serão perdidos quando a aplicação reiniciar. A persistência com MySQL será implementada depois, sem tratar a etapa temporária como se fosse um banco de dados definitivo.

## Estratégia de desenvolvimento

1. Revisar e organizar a estrutura existente do projeto.
2. Definir os modelos e as regras de validação.
3. Finalizar cadastro, login e proteção das rotas privadas.
4. Implementar o catálogo fixo de produtos.
5. Implementar o carrinho e os cálculos de preço.
6. Implementar a finalização dos pedidos e o histórico.
7. Implementar entregadores e atualização do status da entrega.
8. Testar os fluxos e os erros no Postman.
9. Em sala de aula, criar o banco MySQL e integrar a API usando Entity Framework Core, conforme permitido pelo professor.

## Estrutura prevista

Manter a arquitetura simples e compatível com o projeto atual:

- `Model/`: classes que representam os dados da aplicação.
- `Controller/`: endpoints HTTP e respostas da API.
- `Data/`: futura configuração de acesso ao banco e contexto do Entity Framework Core.
- `Service/`: adicionar apenas quando uma regra de negócio justificar separar essa responsabilidade do controller.

A estrutura poderá ser ajustada conforme as necessidades reais, evitando criar camadas sem necessidade.

## Funcionalidades e endpoints planejados

Os caminhos abaixo são uma proposta e podem ser ajustados durante a implementação.

### 1. Usuários e autenticação

- `POST /api/usuario`: cadastrar usuário.
- `POST /api/usuario/login`: autenticar usuário.
- Validar campos obrigatórios e e-mail duplicado.
- Nunca devolver a senha na resposta.
- Não armazenar senhas em texto puro; usar um mecanismo apropriado de hash.
- Proteger as operações privadas e identificar o usuário autenticado sem confiar em um ID enviado livremente pelo frontend.

### 2. Catálogo de produtos

- `GET /api/produto`: listar as rosquinhas disponíveis.
- `GET /api/produto/{id}`: consultar um produto.
- O catálogo será definido pela aplicação. Clientes não poderão cadastrar, alterar ou excluir produtos.
- Preços e disponibilidade serão controlados pelo backend.

### 3. Carrinho

- `GET /api/carrinho`: consultar o carrinho do usuário autenticado.
- `POST /api/carrinho/itens`: adicionar um produto ou aumentar sua quantidade.
- `PUT /api/carrinho/itens/{id}`: alterar a quantidade.
- `DELETE /api/carrinho/itens/{id}`: remover um item.
- Validar produto existente, quantidade válida e limites definidos.
- O total será calculado no backend usando os preços oficiais, nunca valores enviados pelo frontend.

### 4. Pedidos

- `POST /api/pedido`: finalizar a compra e criar um pedido.
- `GET /api/pedido/historico`: listar os pedidos do usuário autenticado.
- `GET /api/pedido/{id}`: consultar um pedido autorizado.
- Registrar os itens, quantidades e preços praticados no momento da compra, para que o histórico não mude se o preço do catálogo for alterado depois.
- Validar carrinho não vazio antes da finalização.

### 5. Entregas

- Associar um entregador a um pedido, conforme as regras definidas para o trabalho.
- Permitir consultar os dados de entrega necessários.
- Controlar os status do pedido/entrega com valores definidos, por exemplo: `Pendente`, `Preparando`, `SaiuParaEntrega` e `Entregue`.
- Restringir quem pode alterar o status, conforme os perfis e as regras combinados com o grupo.

## Armazenamento temporário antes do MySQL

Enquanto o banco não for criado, coleções em memória poderão simular usuários, produtos, carrinhos, pedidos e entregadores. O objetivo é validar a lógica e os contratos HTTP.

- Centralizar o armazenamento temporário para facilitar sua substituição.
- Não espalhar listas estáticas e regras de persistência por vários controllers.
- Não considerar os dados de teste permanentes.
- Ao integrar o MySQL, transferir o acesso aos dados para o contexto/repositórios necessários e criar as relações e tabelas com o grupo.

## Modelo de dados previsto para a etapa do banco

A estrutura final será confirmada antes da criação das tabelas, mas deverá contemplar, no mínimo:

- **Usuario:** dados do cliente e credenciais protegidas.
- **Produto:** nome, descrição opcional, preço e disponibilidade.
- **Pedido:** cliente, datas, status e total.
- **ItemPedido:** produto, quantidade e preço unitário registrado na compra.
- **Entregador:** dados necessários para identificar o responsável pela entrega.
- **Relacionamentos:** um usuário pode ter vários pedidos; um pedido contém vários itens; cada item referencia um produto; um pedido pode estar associado a um entregador.

O carrinho pode ser mantido separadamente ou representado por um pedido em estado de carrinho. Essa decisão será tomada antes de implementar a persistência, considerando a simplicidade exigida pelo projeto escolar.

## Roteiro de testes no Postman

1. Cadastrar um usuário válido.
2. Tentar cadastrar novamente o mesmo e-mail.
3. Testar campos obrigatórios inválidos.
4. Fazer login com credenciais corretas e incorretas.
5. Consultar o catálogo e um produto inexistente.
6. Adicionar produtos ao carrinho e alterar quantidades.
7. Testar remoção e quantidades inválidas.
8. Conferir se o backend calcula o total corretamente.
9. Tentar finalizar um carrinho vazio e um carrinho válido.
10. Consultar o histórico e os detalhes do pedido.
11. Testar associação do entregador e transições de status.
12. Verificar que um usuário não consegue acessar ou alterar dados privados de outro usuário.

## Critério para considerar o backend pronto antes do banco

A etapa pré-MySQL estará concluída quando os endpoints implementados responderem corretamente, as validações e regras de negócio estiverem testadas no Postman e a estrutura estiver preparada para substituir o armazenamento em memória pela persistência real.

A conclusão dessa etapa **não significa** que os dados já são persistentes. Essa parte dependerá da criação e integração do MySQL em sala de aula.
