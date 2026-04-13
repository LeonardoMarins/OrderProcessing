using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Commands.ProcessOrder;

public class ProcessOrderCommandHandler : IRequestHandler<ProcessOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<ProcessOrderCommandHandler> _logger;

    public ProcessOrderCommandHandler(
        IOrderRepository repository,
        ILogger<ProcessOrderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing order {OrderId} for client {Client}", request.Id, request.Client);

        var order = Order.Restore(request.Id, request.Client, request.Value, request.OrderDate);
        await _repository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} saved to database", request.Id);
    }
}
