using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using OrderProcessing.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OrderProcessing.Application.Orders.Commands.CreateOrder;
using OrderProcessing.Infrastructure.Module;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(
    new GuidSerializer(GuidRepresentation.Standard));

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors("FrontendPolicy");
app.MapControllers();

app.Run();