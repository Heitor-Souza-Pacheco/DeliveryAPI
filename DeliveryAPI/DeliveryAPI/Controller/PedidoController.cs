using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context) => _context = context;

    [HttpPost("{clienteId:int}")]
    public async Task<IActionResult> Criar(int clienteId)
    {
        bool clienteExiste = await _context.Usuarios.AnyAsync(u =>
            u.Id == clienteId && u.TipoConta == "Cliente");
        if (!clienteExiste)
            return NotFound("Cliente não encontrado.");

        var carrinho = await _context.ItensCarrinho
            .Where(i => i.UsuarioId == clienteId).ToListAsync();

        if (carrinho.Count == 0)
            return BadRequest("Não é possível criar um pedido com o carrinho vazio.");

        var itensPedido = new List<ItemPedido>();
        foreach (var item in carrinho)
        {
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
            if (produto == null || !produto.Disponivel)
                return BadRequest($"O produto de ID {item.ProdutoId} está indisponível.");

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
            ClienteId = clienteId,
            DataCriacao = DateTime.Now,
            Status = "Recebido",
            Itens = itensPedido
        };

        _context.Pedidos.Add(pedido);
        _context.ItensCarrinho.RemoveRange(carrinho);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { pedidoId = pedido.Id }, pedido);
    }

    [HttpGet("{pedidoId:int}")]
    public async Task<IActionResult> BuscarPorId(int pedidoId)
    {
        var pedido = await _context.Pedidos.Include(p => p.Itens)
            .AsNoTracking().FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
            return NotFound("Pedido não encontrado.");

        return Ok(pedido);
    }

    [HttpGet("cliente/{clienteId:int}")]
    public async Task<IActionResult> ListarPorCliente(int clienteId)
    {
        bool clienteExiste = await _context.Usuarios.AnyAsync(u =>
            u.Id == clienteId && u.TipoConta == "Cliente");
        if (!clienteExiste)
            return NotFound("Cliente não encontrado.");

        var pedidos = await _context.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataCriacao)
            .AsNoTracking().ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("entregador/{entregadorId:int}")]
    public async Task<IActionResult> ListarTodosParaEntregador(int entregadorId)
    {
        bool entregadorExiste = await _context.Usuarios.AnyAsync(u =>
            u.Id == entregadorId && u.TipoConta == "Entregador");
        if (!entregadorExiste)
            return NotFound("Entregador não encontrado.");

        var pedidos = await _context.Pedidos.Include(p => p.Itens)
            .OrderByDescending(p => p.DataCriacao)
            .AsNoTracking().ToListAsync();

        return Ok(pedidos);
    }

    [HttpPut("{pedidoId:int}/status")]
    public async Task<IActionResult> AtualizarStatus(int pedidoId,
        [FromBody] AtualizarStatusPedido requisicao)
    {
        bool entregadorExiste = await _context.Usuarios.AnyAsync(u =>
            u.Id == requisicao.EntregadorId && u.TipoConta == "Entregador");
        if (!entregadorExiste)
            return NotFound("Entregador não encontrado.");

        var pedido = await _context.Pedidos.Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);
        if (pedido == null)
            return NotFound("Pedido não encontrado.");

        bool proximoStatusValido = pedido.Status switch
        {
            "Recebido" => requisicao.Status == "EmPreparacao",
            "EmPreparacao" => requisicao.Status == "SaiuParaEntrega",
            "SaiuParaEntrega" => requisicao.Status == "Entregue",
            _ => false
        };

        if (!proximoStatusValido)
            return BadRequest("Transição de status inválida. O fluxo permitido é Recebido, EmPreparacao, SaiuParaEntrega e Entregue.");

        pedido.Status = requisicao.Status;
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }
}
