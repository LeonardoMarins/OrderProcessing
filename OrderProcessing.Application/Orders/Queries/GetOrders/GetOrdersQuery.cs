using MediatR;
using OrderProcessing.Application.Common;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery(int Page = 1, int PageSize = 10) : IRequest<PagedList<Order>>;

