using DeliveryAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

// [ApiController] ativa comportamentos próprios de uma API, como validação automática do modelo.
// [Route] define o caminho-base. [controller] vira o nome da classe sem "Controller".
[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    [HttpGet] // Atende GET /api/produto.
    public IActionResult Listar()
    {
        return Ok(CatalogoProdutos.Listar()); // Ok envia HTTP 200 com os dados.
    }

    [HttpGet("{id:int}")] // O parâmetro da rota precisa ser um inteiro.
    public IActionResult BuscarPorId(int id)
    {
        var produto = CatalogoProdutos.BuscarPorId(id);

        if (produto == null)
        {
            return NotFound("Produto não encontrado."); // HTTP 404.
        }

        return Ok(produto);
    }
}
