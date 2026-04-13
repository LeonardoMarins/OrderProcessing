using ErrorOr;
using MediatR;

namespace OrderProcessing.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(string Client, decimal Value) : IRequest<ErrorOr<Guid>>;
