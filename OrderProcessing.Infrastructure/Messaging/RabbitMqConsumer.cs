using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Orders.Commands.ProcessOrder;
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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private IChannel? _channel;

    public RabbitMqConsumer(
        IRabbitMqConnection rabbit,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqConsumer> logger)
    {
        _rabbit = rabbit;
        _scopeFactory = scopeFactory;
        _logger = logger;
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
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<OrderMessage>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (message is null)
                    throw new InvalidOperationException("Mensagem inválida recebida na fila.");

                await using var scope = _scopeFactory.CreateAsyncScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ProcessOrderCommand(
                    message.Id,
                    message.Client,
                    message.Value,
                    message.OrderDate), cancellationToken);

                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue");
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
