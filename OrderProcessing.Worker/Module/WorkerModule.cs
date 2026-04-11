using OrderProcessing.Application.Orders.Commands.ProcessOrder;
using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Worker.Module;

public static class WorkerModule
{
    public static IServiceCollection AddWorker(this IServiceCollection services)
    {
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessOrderCommand).Assembly));
        
        services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMqTopologySetup>();

        return services;
    }
}