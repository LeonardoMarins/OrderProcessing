using OrderProcessing.Application.Common;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedList<Order>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
}
