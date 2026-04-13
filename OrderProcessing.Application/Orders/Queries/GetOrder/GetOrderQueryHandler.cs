using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, ErrorOr<Order>>
{
    private readonly IOrderRepository _repository;
    private readonly IMongoCacheService _cache;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(
        IOrderRepository repository,
        IMongoCacheService cache,
        ILogger<GetOrderQueryHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ErrorOr<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(request.Id);

        if (cached is not null)
        {
            _logger.LogInformation("Order {OrderId} retrieved from cache", request.Id);
            return cached;
        }

        _logger.LogInformation("Order {OrderId} not found in cache, querying database", request.Id);

        var order = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.Id);
            return Error.NotFound("Order.NotFound", $"Order {request.Id} was not found.");
        }

        await _cache.SetAsync(order);
        _logger.LogInformation("Order {OrderId} saved to cache", request.Id);

        return order;
    }
}
