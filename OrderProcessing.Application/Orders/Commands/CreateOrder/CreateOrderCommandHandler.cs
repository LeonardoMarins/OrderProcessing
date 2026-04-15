using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<Guid>>
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IMessagePublisher publisher,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Client))
            return Error.Validation("Order.Client", "Client name is required.");

        if (request.Value <= 0)
            return Error.Validation("Order.Value", "Order value must be greater than zero.");

        var order = Order.Restore(request.Id, request.Client, request.Value, request.OrderDate);

        _logger.LogInformation("Publishing order {OrderId} for client {Client}", order.Id, order.Client);

        await _publisher.PublishAsync(order);

        _logger.LogInformation("Order {OrderId} published successfully", order.Id);

        return order.Id;
    }
}
