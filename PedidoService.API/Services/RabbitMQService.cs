using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PedidoService.API.Configuration;
using PedidoService.API.Models;
using PedidoService.API.Models.Events;
using RabbitMQ.Client;

namespace PedidoService.API.Services;

public interface IMessageBusService
{
    void PublishPedidoCriado(PedidoCriadoEvent pedidoCriado);
    void PublishPagamentoConfirmado(PagamentoConfirmadoEvent pagamentoConfirmado);
}

public class RabbitMQService : IMessageBusService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "pedidos_topic_exchange";
    private const string RoutingKey = "pedido.criado";

    public RabbitMQService(IOptions<RabbitMQConfig> config)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(config.Value.Uri)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(
            queue: "pagamentos_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: "pagamentos_queue",
            exchange: ExchangeName,
            routingKey: RoutingKey);
    }

    public void PublishPagamentoConfirmado(PagamentoConfirmadoEvent evento)
    {
        var json = JsonSerializer.Serialize(evento);
        var body = Encoding.UTF8.GetBytes(json);
        
        _channel.BasicPublish(
            exchange: "pedidos_topic_exchange",
            routingKey: "pagamento.confirmado",
            basicProperties: null,
            body: body);
    }

    public void PublishPedidoCriado(PedidoCriadoEvent pedidoCriado)
    {
        try
        {
            var message = JsonSerializer.Serialize(pedidoCriado);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error publishing message to RabbitMQ", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception)
        {
        }
    }
}