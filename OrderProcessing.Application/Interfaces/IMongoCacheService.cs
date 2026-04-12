using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Interfaces;

public interface IMongoCacheService
{
    Task<Order?> GetAsync(Guid id);
    Task SetAsync(Order order);
}