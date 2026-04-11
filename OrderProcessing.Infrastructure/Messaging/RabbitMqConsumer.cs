using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessing.Infrastructure.Messaging;

public interface IRabbitMqConsumer : IAsyncDisposable
{
    Task ConsumeAsync(string queueName, CancellationToken cancellationToken);
}

public class RabbitMqConsumer : IRabbitMqConsumer, IAsyncDisposable
{
    private readonly IRabbitMqConnection _rabbit;
    private IChannel? _channel;

    public RabbitMqConsumer(IRabbitMqConnection rabbit)
    {
        _rabbit = rabbit;
    }

    public async Task ConsumeAsync(string queueName, CancellationToken cancellationToken)
    {
        _channel = await _rabbit.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message);

                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                await _channel.BasicNackAsync(args.DeliveryTag, false, requeue: true, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
    }
}
