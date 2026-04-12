using MediatR;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order?>
{
    private readonly IOrderRepository _repository;
    private readonly IMongoCacheService _cache;

    public GetOrderQueryHandler(IOrderRepository repository,  IMongoCacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(request.Id);

        if (cached is not null)
            return cached;

        var order = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (order is not null)
            await _cache.SetAsync(order);

        return order;
    }
}
