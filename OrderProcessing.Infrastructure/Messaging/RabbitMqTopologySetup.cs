using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace OrderProcessing.Infrastructure.Messaging;

public class RabbitMqTopologySetup : IHostedService
{
    private readonly IRabbitMqConnection _rabbit;
    private readonly RabbitMqSettings _settings;

    public RabbitMqTopologySetup(IRabbitMqConnection rabbit, IOptions<RabbitMqSettings> options)
    {
        _rabbit = rabbit;
        _settings = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _rabbit.InitializeAsync();

        await using var channel = await _rabbit.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null, 
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
