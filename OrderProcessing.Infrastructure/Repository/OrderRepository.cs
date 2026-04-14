using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Common;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders.FindAsync([id], cancellationToken);
    }

    public async Task<PagedList<Order>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await _context.Orders.CountAsync(cancellationToken);

        var items = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Order>(items, page, pageSize, totalCount);
    }
}
