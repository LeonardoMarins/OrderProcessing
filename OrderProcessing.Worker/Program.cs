using OrderProcessing.Infrastructure.Module;
using OrderProcessing.Worker;
using OrderProcessing.Worker.Module;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWorker();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();