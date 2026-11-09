using DeliveryAPI.Data;
using DeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrinhoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarrinhoController(AppDbContext context) => _context = context;

    [HttpGet("{usuarioId:int}")]
    public async Task<IActionResult> Consultar(int usuarioId)
    {
        if (!await ClienteExiste(usuarioId))
            return NotFound("Cliente não encontrado.");

        return Ok(await MontarRespostaCarrinho(usuarioId));
    }

    [HttpPost("{usuarioId:int}/itens")]
    public async Task<IActionResult> Adicionar(int usuarioId, [FromBody] ItemCarrinho item)
    {
        if (!await ClienteExiste(usuarioId))
            return NotFound("Cliente não encontrado.");

        var produto = await _context.Produtos.FindAsync(item.ProdutoId);
        if (produto == null || !produto.Disponivel)
            return NotFound("Produto não encontrado ou indisponível.");

        var itemExistente = await _context.ItensCarrinho.FindAsync(usuarioId, item.ProdutoId);
        int novaQuantidade = (itemExistente?.Quantidade ?? 0) + item.Quantidade;

        if (novaQuantidade > 100)
            return BadRequest("A quantidade total de um produto não pode ultrapassar 100.");

        if (itemExistente == null)
            _context.ItensCarrinho.Add(new ItemCarrinho
            {
                UsuarioId = usuarioId,
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            });
        else
            itemExistente.Quantidade = novaQuantidade;

        await _context.SaveChangesAsync();
        return Ok(await MontarRespostaCarrinho(usuarioId));
    }

    [HttpPut("{usuarioId:int}/itens/{produtoId:int}")]
    public async Task<IActionResult> AtualizarQuantidade(int usuarioId, int produtoId,
        [FromBody] QuantidadeCarrinho requisicao)
    {
        if (!await ClienteExiste(usuarioId))
            return NotFound("Cliente não encontrado.");

        var item = await _context.ItensCarrinho.FindAsync(usuarioId, produtoId);
        if (item == null)
            return NotFound("Esse produto não está no carrinho.");

        item.Quantidade = requisicao.Quantidade;
        await _context.SaveChangesAsync();
        return Ok(await MontarRespostaCarrinho(usuarioId));
    }

    [HttpDelete("{usuarioId:int}/itens/{produtoId:int}")]
    public async Task<IActionResult> Remover(int usuarioId, int produtoId)
    {
        if (!await ClienteExiste(usuarioId))
            return NotFound("Cliente não encontrado.");

        var item = await _context.ItensCarrinho.FindAsync(usuarioId, produtoId);
        if (item == null)
            return NotFound("Esse produto não está no carrinho.");

        _context.ItensCarrinho.Remove(item);
        await _context.SaveChangesAsync();
        return Ok(await MontarRespostaCarrinho(usuarioId));
    }

    private async Task<bool> ClienteExiste(int usuarioId)
    {
        return await _context.Usuarios.AnyAsync(u =>
            u.Id == usuarioId && u.TipoConta == "Cliente");
    }

    private async Task<object> MontarRespostaCarrinho(int usuarioId)
    {
        var itens = await _context.ItensCarrinho
            .Where(i => i.UsuarioId == usuarioId)
            .Join(_context.Produtos, i => i.ProdutoId, p => p.Id,
                (i, p) => new
                {
                    produtoId = i.ProdutoId,
                    nome = p.Nome,
                    precoUnitario = p.Preco,
                    quantidade = i.Quantidade,
                    subtotal = p.Preco * i.Quantidade
                })
            .ToListAsync();

        return new { usuarioId, itens, total = itens.Sum(i => i.subtotal) };
    }
}
