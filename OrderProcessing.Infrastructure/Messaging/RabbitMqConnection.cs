using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
namespace OrderProcessing.Infrastructure.Messaging;

public interface IRabbitMqConnection
{
    public Task InitializeAsync();
    Task<IChannel> CreateChannelAsync();
}

public class RabbitMqConnection : IRabbitMqConnection
{
    private IConnection? _connection;
    private readonly RabbitMqSettings _settings;
    
    public RabbitMqConnection(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Port = _settings.Port,
            VirtualHost = _settings.UserName
        };

        _connection = await factory.CreateConnectionAsync();
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        if (_connection is null)
            throw new InvalidOperationException("RabbitMQ not initialized.");

        return await _connection.CreateChannelAsync();
    }
}
