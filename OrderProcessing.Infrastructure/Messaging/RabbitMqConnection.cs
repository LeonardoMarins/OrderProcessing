using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessing.Infrastructure.Messaging;

public interface IRabbitMqConnection
{
    public Task InitializeAsync();
    Task PublishAsync(string queue, byte[] body);
    Task SubscribeAsync(string queue);
}

public class RabbitMqConnection : IRabbitMqConnection
{
    private IConnection _connection;
    private IChannel _channel;

    private async Task ConnectAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
        };

        _connection = await factory.CreateConnectionAsync();
    }

    private async Task CreateChannelAsync()
    {
        _channel = await _connection.CreateChannelAsync();
    }

    private async Task DeclareQueueAsync()
    {
        await _channel.QueueDeclareAsync(queue: "Order",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public async Task InitializeAsync()
    {
        await ConnectAsync();
        await CreateChannelAsync();
        await DeclareQueueAsync();
    }

    public async Task PublishAsync(string queue, byte[] body)
    {
         await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                body: body);
    }

    public async Task SubscribeAsync(string queue)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // processa mensagem
            Console.WriteLine(message);

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(
            queue: queue,
            autoAck: true,
            consumer: consumer);
    }
}