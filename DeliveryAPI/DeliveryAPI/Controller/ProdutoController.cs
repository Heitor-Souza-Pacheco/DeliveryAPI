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

    [HttpGet("{id}")]
public IActionResult BuscarPorId(string id)
{
    if (!int.TryParse(id, out int idConvertido))
    {
        return BadRequest("O ID precisa ser um número inteiro.");
    }

    var produto = CatalogoProdutos.BuscarPorId(idConvertido);

    if (produto == null)
    {
        return NotFound("Produto não encontrado.");
    }

    return Ok(produto);
}
}
