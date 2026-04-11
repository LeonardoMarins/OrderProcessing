using MediatR;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
