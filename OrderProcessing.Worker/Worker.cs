using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Worker;

public class Worker : BackgroundService
{
    private readonly RabbitMqConsumer _consumer;
    private readonly IRabbitMqConnection _rabbit;

    public Worker(
        IRabbitMqConnection rabbit,
        RabbitMqConsumer consumer)
    {
        _rabbit = rabbit;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbit.InitializeAsync();
        await _consumer.ConsumeAsync("Order");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}