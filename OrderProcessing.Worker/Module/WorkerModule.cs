using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Worker.Module;

public static class WorkerModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddScoped<IRabbitMqConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMqTopologySetup>();

        return services;
    }
}