using MediatR;
using OrderProcessing.Application.Common;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedList<Order>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedList<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
    }
}
