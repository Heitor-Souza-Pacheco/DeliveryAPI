using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    [HttpPost("{clienteId:int}")]
    public IActionResult Criar(int clienteId)
    {
        var cliente = ArmazenamentoTemporario.Usuarios.FirstOrDefault(usuario =>
            usuario.Id == clienteId &&
            usuario.TipoConta.Equals("Cliente", StringComparison.OrdinalIgnoreCase));

        if (cliente == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        var carrinho = CarrinhosTemporarios.ObterOuCriar(clienteId);

        if (carrinho.Count == 0)
        {
            return BadRequest("Não é possível criar um pedido com o carrinho vazio.");
        }

        var itensPedido = new List<ItemPedido>();

        foreach (var item in carrinho)
        {
            var produto = CatalogoProdutos.BuscarPorId(item.ProdutoId);

            if (produto == null || !produto.Disponivel)
            {
                return BadRequest($"O produto de ID {item.ProdutoId} está indisponível.");
            }

            itensPedido.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                PrecoUnitario = produto.Preco,
                Quantidade = item.Quantidade
            });
        }

        var pedido = new Pedido
        {
            Id = PedidosTemporarios.GerarProximoId(),
            ClienteId = clienteId,
            DataCriacao = DateTime.Now,
            Status = "Recebido",
            Itens = itensPedido
        };

        PedidosTemporarios.Pedidos.Add(pedido);
        carrinho.Clear();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { pedidoId = pedido.Id },
            pedido);
    }

    [HttpGet("{pedidoId:int}")]
    public IActionResult BuscarPorId(int pedidoId)
    {
        var pedido = PedidosTemporarios.Pedidos.FirstOrDefault(p => p.Id == pedidoId);

        if (pedido == null)
        {
            return NotFound("Pedido não encontrado.");
        }

        return Ok(pedido);
    }

    [HttpGet("cliente/{clienteId:int}")]
    public IActionResult ListarPorCliente(int clienteId)
    {
        var cliente = ArmazenamentoTemporario.Usuarios.FirstOrDefault(usuario =>
            usuario.Id == clienteId &&
            usuario.TipoConta.Equals("Cliente", StringComparison.OrdinalIgnoreCase));

        if (cliente == null)
        {
            return NotFound("Cliente não encontrado.");
        }

        var pedidos = PedidosTemporarios.Pedidos
            .Where(pedido => pedido.ClienteId == clienteId)
            .OrderByDescending(pedido => pedido.DataCriacao)
            .ToList();

        return Ok(pedidos);
    }

    [HttpPut("{pedidoId:int}/status")]
    public IActionResult AtualizarStatus(int pedidoId, [FromBody] AtualizarStatusPedido requisicao)
    {
        var entregador = ArmazenamentoTemporario.Usuarios.FirstOrDefault(usuario =>
            usuario.Id == requisicao.EntregadorId &&
            usuario.TipoConta.Equals("Entregador", StringComparison.OrdinalIgnoreCase));

        if (entregador == null)
        {
            return NotFound("Entregador não encontrado.");
        }

        var pedido = PedidosTemporarios.Pedidos.FirstOrDefault(p => p.Id == pedidoId);

        if (pedido == null)
        {
            return NotFound("Pedido não encontrado.");
        }

        var statusAtual = pedido.Status;
        var proximoStatusValido = statusAtual switch
        {
            "Recebido" => requisicao.Status == "EmPreparacao",
            "EmPreparacao" => requisicao.Status == "SaiuParaEntrega",
            "SaiuParaEntrega" => requisicao.Status == "Entregue",
            _ => false
        };

        if (!proximoStatusValido)
        {
            return BadRequest("Transição de status inválida. O fluxo permitido é Recebido, EmPreparacao, SaiuParaEntrega e Entregue.");
        }

        pedido.Status = requisicao.Status;

        return Ok(pedido);
    }
}
