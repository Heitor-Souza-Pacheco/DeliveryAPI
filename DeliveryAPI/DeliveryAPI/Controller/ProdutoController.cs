using DeliveryAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _context.Produtos.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var produto = await _context.Produtos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound("Produto não encontrado.");

        return Ok(produto);
    }
}
