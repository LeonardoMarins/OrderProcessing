using Microsoft.Extensions.Options;
using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Worker;

public class Worker : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;
    private readonly RabbitMqSettings _settings;

    public Worker(
        IRabbitMqConsumer consumer,
        IOptions<RabbitMqSettings> options)
    {
        _consumer = consumer;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync(
            _settings.QueueName,
            stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

