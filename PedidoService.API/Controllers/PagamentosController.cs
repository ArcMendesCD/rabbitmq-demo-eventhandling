using Microsoft.AspNetCore.Mvc;
using PedidoService.API.Models;
using PedidoService.API.Models.Events;
using PedidoService.API.Services;

namespace PedidoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController : ControllerBase
{
    private readonly IMessageBusService _messageBusService;
    private readonly IPagamentoService _pagamentoService;

    public PagamentosController(
        IMessageBusService messageBusService,
        IPagamentoService pagamentoService)
    {
        _messageBusService = messageBusService;
        _pagamentoService = pagamentoService;
    }

    [HttpPost("processar")]
    public IActionResult ProcessarPagamento([FromBody] ProcessarPagamentoRequest request)
    {
        try
        {
            var pedidoId = Guid.NewGuid().ToString("N");

            var pedidoCriado = new PedidoCriadoEvent
            {
                pedido_id = pedidoId,
                cliente_id = request.ClienteId,
                valor_total = request.ValorTotal,
                itens = request.Itens.Select(i => new ItemPedido
                {
                    produto_id = i.ProdutoId,
                    quantidade = i.Quantidade
                }).ToList()
            };

            _messageBusService.PublishPedidoCriado(pedidoCriado);

            return Ok(new 
            { 
                mensagem = "Pedido enviado para processamento", 
                pedidoId = pedidoId 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = $"Erro ao processar pagamento: {ex.Message}" });
        }
    }

   
}