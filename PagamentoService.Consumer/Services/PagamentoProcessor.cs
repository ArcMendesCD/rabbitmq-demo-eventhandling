using PagamentoService.Consumer.Models;
using PagamentoService.Consumer.Messaging;

namespace PagamentoService.Consumer.Services;

public class PagamentoProcessor
{
    private readonly RabbitMQPublisher _publisher;

    public PagamentoProcessor(RabbitMQPublisher publisher)
    {
        _publisher = publisher;
    }

    public void ProcessarPagamento(PedidoCriadoEvent pedido)
    {
        Console.WriteLine($"[Pagamento] Processando pedido {pedido.pedido_id}...");

        Thread.Sleep(1000);

        var pagamento = new PagamentoConfirmadoEvent
        {
            pedido_id = pedido.pedido_id,
            valor_pago = pedido.valor_total,
            data_confirmacao = DateTime.UtcNow
        };

        _publisher.Publicar("pagamento.confirmado", pagamento);
        Console.WriteLine($"[Pagamento] Confirmado e publicado: {pagamento.pedido_id}");
    }
}