using ErrorOr;
using MediatR;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Queries.GetOrder;

public record GetOrderQuery(Guid Id) : IRequest<ErrorOr<Order>>;
