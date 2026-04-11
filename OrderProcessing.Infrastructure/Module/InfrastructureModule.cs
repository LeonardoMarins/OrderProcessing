using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Infrastructure.Module;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //DB
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
        
        //RabbitMQ
        services.Configure<RabbitMqSettings>(
            configuration.GetSection("RabbitMq"));
        
        //DI
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddScoped<IMessagePublisher, RabbitMqProducer>();
        services.AddScoped<IRabbitMqConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMqTopologySetup>();

        return services;
    }
}