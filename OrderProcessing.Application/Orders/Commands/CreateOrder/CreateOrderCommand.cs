using MediatR;

namespace OrderProcessing.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid Id, string Client, decimal Value, DateTime OrderData) : IRequest<Guid>;
