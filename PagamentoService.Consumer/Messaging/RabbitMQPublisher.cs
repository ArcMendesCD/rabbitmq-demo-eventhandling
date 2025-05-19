using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PagamentoService.Consumer.Messaging;

public class RabbitMQPublisher
{
    private readonly IModel _channel;

    public RabbitMQPublisher(IModel channel)
    {
        _channel = channel;
    }

    public void Publicar(string routingKey, object mensagem)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));
        _channel.BasicPublish("pedidos_topic_exchange", routingKey, null, body);
    }
}