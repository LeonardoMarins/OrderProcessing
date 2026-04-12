using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Infrastructure.Caching;

public class MongoCacheService : IMongoCacheService
{
    private readonly IMongoCollection<Order> _collection;

    public MongoCacheService(
        IMongoClient client,
        IOptions<MongoCacheSettings> options)
    {
        var settings = options.Value;

        var database = client.GetDatabase(settings.DatabaseName);

        _collection = database.GetCollection<Order>(
            settings.CollectionName);
    }

    public async Task<Order?> GetAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task SetAsync(Order order)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == order.Id,
            order,
            new ReplaceOptions { IsUpsert = true });
    }
}