using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

// ControllerBase fornece métodos prontos para responder HTTP, como Ok e NotFound.
[ApiController]
[Route("api/[controller]")]
public class CarrinhoController : ControllerBase
{
    [HttpGet("{usuarioId:int}")] // GET /api/carrinho/{usuarioId}; :int restringe a rota a inteiros.
    public IActionResult Consultar(int usuarioId)
    {
        var usuario = BuscarCliente(usuarioId);

        if (usuario == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        return Ok(MontarRespostaCarrinho(usuarioId));
    }

    [HttpPost("{usuarioId:int}/itens")]
    public IActionResult Adicionar(int usuarioId, [FromBody] ItemCarrinho item)
    {
        var usuario = BuscarCliente(usuarioId);

        if (usuario == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        var produto = CatalogoProdutos.BuscarPorId(item.ProdutoId);

        if (produto == null || !produto.Disponivel)
        {
            return NotFound("Produto não encontrado ou indisponível.");
        }

        var carrinho = CarrinhosTemporarios.ObterOuCriar(usuarioId);

        // FirstOrDefault procura o item; se não encontrar, retorna null.
        var itemExistente = carrinho.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);

        // ?. acessa Quantidade apenas se itemExistente não for null; ?? usa 0 como alternativa.
        var novaQuantidade = (itemExistente?.Quantidade ?? 0) + item.Quantidade;

        if (novaQuantidade > 100)
        {
            return BadRequest("A quantidade total de um produto não pode ultrapassar 100.");
        }

        if (itemExistente == null)
        {
            carrinho.Add(new ItemCarrinho
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            });
        }
        else
        {
            itemExistente.Quantidade = novaQuantidade;
        }

        return Ok(MontarRespostaCarrinho(usuarioId));
    }

    [HttpPut("{usuarioId:int}/itens/{produtoId:int}")]
    public IActionResult AtualizarQuantidade(
        int usuarioId,
        int produtoId,
        [FromBody] QuantidadeCarrinho requisicao)
    {
        var usuario = BuscarCliente(usuarioId);

        if (usuario == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        var carrinho = CarrinhosTemporarios.ObterOuCriar(usuarioId);
        var item = carrinho.FirstOrDefault(i => i.ProdutoId == produtoId);

        if (item == null)
        {
            return NotFound("Esse produto não está no carrinho.");
        }

        item.Quantidade = requisicao.Quantidade;

        return Ok(MontarRespostaCarrinho(usuarioId));
    }

    [HttpDelete("{usuarioId:int}/itens/{produtoId:int}")]
    public IActionResult Remover(int usuarioId, int produtoId)
    {
        var usuario = BuscarCliente(usuarioId);

        if (usuario == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        var carrinho = CarrinhosTemporarios.ObterOuCriar(usuarioId);
        var item = carrinho.FirstOrDefault(i => i.ProdutoId == produtoId);

        if (item == null)
        {
            return NotFound("Esse produto não está no carrinho.");
        }

        carrinho.Remove(item);

        return Ok(MontarRespostaCarrinho(usuarioId));
    }

    // private: método usado só dentro desta classe. static: não depende de um objeto controller.
    // O ? em Usuario? indica que o método pode não encontrar um cliente e retornar null.
    private static Usuario? BuscarCliente(int usuarioId)
    {
        return ArmazenamentoTemporario.Usuarios.FirstOrDefault(
            usuario => usuario.Id == usuarioId &&
                       usuario.TipoConta.Equals("Cliente", StringComparison.OrdinalIgnoreCase));
    }

    private static object MontarRespostaCarrinho(int usuarioId)
    {
        // Select transforma cada item do carrinho em dados de resposta.
        var itens = CarrinhosTemporarios.ObterOuCriar(usuarioId)
            .Select(item =>
            {
                var produto = CatalogoProdutos.BuscarPorId(item.ProdutoId);

                // É um objeto anônimo: cria uma resposta sem precisar de outra classe de modelo.
                // ?. e ?? fornecem valores alternativos se o produto não for encontrado.
                return new
                {
                    produtoId = item.ProdutoId,
                    nome = produto?.Nome ?? "Produto não encontrado",
                    precoUnitario = produto?.Preco ?? 0,
                    quantidade = item.Quantidade,
                    subtotal = (produto?.Preco ?? 0) * item.Quantidade
                };
            })
            .ToList(); // Materializa o resultado como uma lista.

        return new
        {
            usuarioId,
            itens,
            total = itens.Sum(item => item.subtotal) // Sum soma todos os subtotais.
        };
    }
}
