using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrderProcessing.Application.Interfaces;
using RabbitMQ.Client;

namespace OrderProcessing.Infrastructure.Messaging;

public class RabbitMqProducer : IMessagePublisher, IAsyncDisposable
{
    private readonly IRabbitMqConnection _rabbit;
    private readonly RabbitMqSettings _settings;
    private IChannel? _channel;

    public RabbitMqProducer(IRabbitMqConnection rabbit, IOptions<RabbitMqSettings> options)
    {
        _rabbit = rabbit;
        _settings = options.Value;
    }

    public async Task PublishAsync<T>(T entity)
    {
        _channel ??= await _rabbit.CreateChannelAsync();

        var message = JsonSerializer.Serialize(entity);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: _settings.QueueName,
            mandatory: false,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
    }
}
