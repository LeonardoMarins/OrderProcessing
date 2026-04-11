namespace OrderProcessing.Infrastructure.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; }
    public string QueueName { get; set; }
}