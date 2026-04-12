using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Orders.Commands.CreateOrder;
using OrderProcessing.Infrastructure.Caching;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Messaging;

namespace OrderProcessing.Infrastructure.Module;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
        
        services.Configure<RabbitMqSettings>(
            configuration.GetSection("RabbitMq"));
        
        services.Configure<MongoCacheSettings>(
            configuration.GetSection("MongoCache"));
        
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp
                .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<MongoCacheSettings>>()
                .Value;

            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<IMongoCacheService, MongoCacheService>();
        
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddScoped<IMessagePublisher, RabbitMqProducer>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddHostedService<RabbitMqTopologySetup>();

        return services;
    }
}