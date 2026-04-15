using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Orders.Commands.CreateOrder;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.UnitTests.Orders;

public class CreateOrderCommandHandlerTests
{
    private readonly IMessagePublisher _publisher = Substitute.For<IMessagePublisher>();
    private readonly ILogger<CreateOrderCommandHandler> _logger = Substitute.For<ILogger<CreateOrderCommandHandler>>();
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(_publisher, _logger);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsGuid()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), "Cliente Teste", 100.00m, DateTime.UtcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesMessage()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), "Cliente Teste", 100.00m, DateTime.UtcNow);

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).PublishAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_EmptyClient_ReturnsValidationError()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), "", 100.00m, DateTime.UtcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_ZeroValue_ReturnsValidationError()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), "Cliente Teste", 0m, DateTime.UtcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_NegativeValue_ReturnsValidationError()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), "Cliente Teste", -10m, DateTime.UtcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }
}
