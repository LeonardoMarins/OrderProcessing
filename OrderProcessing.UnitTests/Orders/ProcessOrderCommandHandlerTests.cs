using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Orders.Commands.ProcessOrder;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.UnitTests.Orders;

public class ProcessOrderCommandHandlerTests
{
    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly ILogger<ProcessOrderCommandHandler> _logger = Substitute.For<ILogger<ProcessOrderCommandHandler>>();
    private readonly ProcessOrderCommandHandler _handler;

    public ProcessOrderCommandHandlerTests()
    {
        _handler = new ProcessOrderCommandHandler(_repository, _logger);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesOrderToRepository()
    {
        var command = new ProcessOrderCommand(Guid.NewGuid(), "Cliente", 100m, DateTime.UtcNow);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_RestoresOrderWithCorrectData()
    {
        var id = Guid.NewGuid();
        var client = "Cliente Teste";
        var value = 150m;
        var orderDate = DateTime.UtcNow;
        Order? savedOrder = null;

        await _repository.AddAsync(Arg.Do<Order>(o => savedOrder = o), Arg.Any<CancellationToken>());

        var command = new ProcessOrderCommand(id, client, value, orderDate);
        await _handler.Handle(command, CancellationToken.None);

        savedOrder.Should().NotBeNull();
        savedOrder!.Id.Should().Be(id);
        savedOrder.Client.Should().Be(client);
        savedOrder.Value.Should().Be(value);
        savedOrder.OrderDate.Should().Be(orderDate);
    }
}
