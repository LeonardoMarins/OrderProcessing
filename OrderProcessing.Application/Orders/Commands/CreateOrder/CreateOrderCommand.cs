using ErrorOr;
using MediatR;

namespace OrderProcessing.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid Id, string Client, decimal Value, DateTime OrderDate) : IRequest<ErrorOr<Guid>>;
