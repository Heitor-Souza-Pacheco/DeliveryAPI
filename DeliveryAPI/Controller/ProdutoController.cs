using DeliveryAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(CatalogoProdutos.Listar());
    }

    [HttpGet("{id:int}")]
    public IActionResult BuscarPorId(int id)
    {
        var produto = CatalogoProdutos.BuscarPorId(id);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        return Ok(produto);
    }
}
