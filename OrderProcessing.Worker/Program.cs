using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderProcessing.Infrastructure.Module;
using OrderProcessing.Worker;
using OrderProcessing.Worker.Module;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

    var appInsightsConnection = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrEmpty(appInsightsConnection))
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("OrderProcessing.Worker"))
            .WithTracing(t => t.AddAzureMonitorTraceExporter(o =>
                o.ConnectionString = appInsightsConnection));
    }

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddWorker();
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
