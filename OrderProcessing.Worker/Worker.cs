using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;

    public Worker(IServiceScopeFactory scopeFactory, IOptions<RabbitMqSettings> options)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumer>();
        await consumer.ConsumeAsync(_settings.QueueName, stoppingToken);
    }
}

