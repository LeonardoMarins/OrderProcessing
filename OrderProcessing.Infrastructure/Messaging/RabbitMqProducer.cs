using System.Text;
using System.Text.Json;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Infrastructure.Messaging;

public class RabbitMqProducer : IMessagePublisher
{
    private readonly IRabbitMqConnection _rabbit;

    public RabbitMqProducer(IRabbitMqConnection rabbit)
    {
        _rabbit = rabbit;
    }

    public async Task PublishAsync<T>(T entity)
    {
        var message = JsonSerializer.Serialize(entity);
        var body = Encoding.UTF8.GetBytes(message);

        await _rabbit.PublishAsync("Order", body);
    }
}