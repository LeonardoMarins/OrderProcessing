using MediatR;

namespace OrderProcessing.Application.Orders.Commands.ProcessOrder;

public record ProcessOrderCommand(Guid Id, string Client, decimal Value, DateTime OrderDate) : IRequest;
