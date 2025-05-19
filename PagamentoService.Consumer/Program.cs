using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PagamentoService.Consumer.Models;
using PagamentoService.Consumer.Messaging;
using PagamentoService.Consumer.Services;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://xzhfkjrz:Kq0gWWMiMN5xR9m1Rmz1kE89Go8ALf4h@porpoise.rmq.cloudamqp.com/xzhfkjrz")
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("pedidos_topic_exchange", ExchangeType.Topic, true);
channel.QueueDeclare("pagamentos_queue", true, false, false);
channel.QueueBind("pagamentos_queue", "pedidos_topic_exchange", "pedido.criado");

channel.QueueDeclare("notificacoes_queue", true, false, false);
channel.QueueBind("notificacoes_queue", "pedidos_topic_exchange", "pagamento.confirmado");


var publisher = new RabbitMQPublisher(channel);
var processor = new PagamentoProcessor(publisher);

var pagamentosConsumer = new EventingBasicConsumer(channel);
pagamentosConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    
    try
    {
        var pedido = JsonSerializer.Deserialize<PedidoCriadoEvent>(json);

        if (pedido != null)
        {
            Console.WriteLine($"[pagamentos_queue] Recebido pedido: {pedido.pedido_id}");
            processor.ProcessarPagamento(pedido);
        }
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"[Erro] Não foi possível processar a mensagem: {ex.Message}");
        Console.WriteLine($"Conteúdo da mensagem: {json}");
    }
};

var notificacoesConsumer = new EventingBasicConsumer(channel);
notificacoesConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    
    try
    {
        var pagamento = JsonSerializer.Deserialize<PagamentoConfirmadoEvent>(json);

        if (pagamento != null)
        {
            Console.WriteLine($"[notificacoes_queue] Pagamento confirmado para o pedido: {pagamento.pedido_id}");
        }
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"[Erro] Não foi possível processar a notificação: {ex.Message}");
    }
};

channel.BasicConsume(queue: "pagamentos_queue", autoAck: true, consumer: pagamentosConsumer);
channel.BasicConsume(queue: "notificacoes_queue", autoAck: true, consumer: notificacoesConsumer);

Console.WriteLine("[pagamentos_queue] Aguardando mensagens...");
Console.ReadLine();