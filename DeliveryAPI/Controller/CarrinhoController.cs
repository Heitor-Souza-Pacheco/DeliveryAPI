using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrinhoController : ControllerBase
{
    [HttpGet("{usuarioId:int}")]
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
        var itemExistente = carrinho.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);
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

    private static Usuario? BuscarCliente(int usuarioId)
    {
        return ArmazenamentoTemporario.Usuarios.FirstOrDefault(
            usuario => usuario.Id == usuarioId &&
                       usuario.TipoConta.Equals("Cliente", StringComparison.OrdinalIgnoreCase));
    }

    private static object MontarRespostaCarrinho(int usuarioId)
    {
        var itens = CarrinhosTemporarios.ObterOuCriar(usuarioId)
            .Select(item =>
            {
                var produto = CatalogoProdutos.BuscarPorId(item.ProdutoId);

                return new
                {
                    produtoId = item.ProdutoId,
                    nome = produto?.Nome ?? "Produto não encontrado",
                    precoUnitario = produto?.Preco ?? 0,
                    quantidade = item.Quantidade,
                    subtotal = (produto?.Preco ?? 0) * item.Quantidade
                };
            })
            .ToList();

        return new
        {
            usuarioId,
            itens,
            total = itens.Sum(item => item.subtotal)
        };
    }
}
