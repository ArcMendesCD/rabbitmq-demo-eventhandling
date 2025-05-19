using PedidoService.API.Controllers;
using PedidoService.API.Models;
using PedidoService.API.Models.Events;

namespace PedidoService.API.Services;

public interface IPagamentoService
{
    Task<bool> ProcessarPagamentoAsync(string pedidoId, decimal valor);
    Task<bool> ConfirmarPagamentoAsync(string pedidoId, decimal valorPago);
}

public class PagamentoService : IPagamentoService
{
    private readonly IMessageBusService _messageBusService;
    private readonly IPedidoService _pedidoService;

    public PagamentoService(IMessageBusService messageBusService, IPedidoService pedidoService)
    {
        _messageBusService = messageBusService;
        _pedidoService = pedidoService;
    }

    public async Task<bool> ProcessarPagamentoAsync(string pedidoId, decimal valor)
    {
        try
        {
            
            await ConfirmarPagamentoAsync(pedidoId, valor);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ConfirmarPagamentoAsync(string pedidoId, decimal valorPago)
    {
        try
        {
            var pagamentoConfirmado = new PagamentoConfirmadoEvent
            {
                pedido_id = pedidoId,
                valor_pago = valorPago,
                status_pagamento = "confirmado",
                data_confirmacao = DateTime.UtcNow
            };

            _messageBusService.PublishPagamentoConfirmado(pagamentoConfirmado);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}