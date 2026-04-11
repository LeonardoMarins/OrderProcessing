namespace OrderProcessing.Infrastructure.Messaging;

public class RabbitMqConsumer
{
    private readonly IRabbitMqConnection _rabbit;

    public RabbitMqConsumer(IRabbitMqConnection rabbit)
    {
        _rabbit = rabbit;
    }

    public async Task ConsumeAsync(string queueName)
    {
        await _rabbit.SubscribeAsync(queueName);
    }
}