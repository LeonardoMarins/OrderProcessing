using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Infrastructure.Caching;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Messaging;
using Testcontainers.MsSql;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;

namespace OrderProcessing.IntegrationTests.Infrastructure;

public class OrderProcessingWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithPassword("Order@2024!")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7")
        .Build();

    private readonly RabbitMqContainer _rabbitContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-alpine")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _sqlContainer.StartAsync(),
            _mongoContainer.StartAsync(),
            _rabbitContainer.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();
        await _rabbitContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_sqlContainer.GetConnectionString()));

            services.Configure<RabbitMqSettings>(opts =>
            {
                opts.HostName = _rabbitContainer.Hostname;
                opts.Port = _rabbitContainer.GetMappedPublicPort(5672);
                opts.UserName = "guest";
                opts.Password = "guest";
                opts.VirtualHost = "/";
                opts.QueueName = "orders";
            });

            services.Configure<MongoCacheSettings>(opts =>
            {
                opts.ConnectionString = _mongoContainer.GetConnectionString();
                opts.DatabaseName = "OrderProcessingCacheTest";
                opts.CollectionName = "Orders";
            });
        });
    }
}
