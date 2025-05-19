using PedidoService.API.Models;
using PedidoService.API.Models.Events;

namespace PedidoService.API.Services;

public interface IPedidoService
{
    Task<PedidoCriadoResponse> CriarPedidoAsync(CriarPedidoRequest request);
}

public class PedidoService : IPedidoService
{
    private readonly IMessageBusService _messageBusService;

    public PedidoService(IMessageBusService messageBusService)
    {
        _messageBusService = messageBusService;
    }

    public Task<PedidoCriadoResponse> CriarPedidoAsync(CriarPedidoRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var pedidoId = Guid.NewGuid().ToString();

        var pedidoCriado = new PedidoCriadoEvent
        {
            pedido_id = pedidoId,
            cliente_id = request.ClienteId,
            valor_total = request.ValorTotal,
            itens = request.Itens.Select(item => new ItemPedido
            {
                produto_id = item.ProdutoId,
                quantidade = item.Quantidade
            }).ToList()
        };

        _messageBusService.PublishPedidoCriado(pedidoCriado);

        return Task.FromResult(new PedidoCriadoResponse
        {
            PedidoId = pedidoId,
            Status = "Pedido criado com sucesso"
        });
    }
}