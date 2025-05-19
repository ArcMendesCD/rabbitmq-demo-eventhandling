using Microsoft.AspNetCore.Mvc;
using PedidoService.API.Models;
using PedidoService.API.Services;

namespace PedidoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<ActionResult<PedidoCriadoResponse>> CriarPedido([FromBody] CriarPedidoRequest request)
    {
        try
        {
            var response = await _pedidoService.CriarPedidoAsync(request);
            return Created($"/api/pedidos/{response.PedidoId}", response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}