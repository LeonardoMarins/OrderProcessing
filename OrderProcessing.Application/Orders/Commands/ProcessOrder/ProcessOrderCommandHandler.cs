using MediatR;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Application.Orders.Commands.ProcessOrder;

public class ProcessOrderCommandHandler : IRequestHandler<ProcessOrderCommand>
{
    private readonly IOrderRepository _repository;

    public ProcessOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Restore(request.Id, request.Client, request.Value, request.OrderDate);
        await _repository.AddAsync(order, cancellationToken);
    }
}
