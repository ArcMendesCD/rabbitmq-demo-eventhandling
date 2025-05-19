using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace PedidoService.API.Messaging;

public interface IMessagePublisher
{
    void Publish<T>(string routingKey, T message);
}

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private const string ExchangeName = "pedidos_topic_exchange";

    public RabbitMQPublisher(IConnection connection)
    {
        _connection = connection;
        _channel = connection.CreateModel();
        
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, true);
    }

    public void Publish<T>(string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(ExchangeName, routingKey, null, body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}