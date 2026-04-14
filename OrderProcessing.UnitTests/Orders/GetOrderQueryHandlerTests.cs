using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Orders.Queries.GetOrder;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.UnitTests.Orders;

public class GetOrderQueryHandlerTests
{
    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly IMongoCacheService _cache = Substitute.For<IMongoCacheService>();
    private readonly ILogger<GetOrderQueryHandler> _logger = Substitute.For<ILogger<GetOrderQueryHandler>>();
    private readonly GetOrderQueryHandler _handler;

    public GetOrderQueryHandlerTests()
    {
        _handler = new GetOrderQueryHandler(_repository, _cache, _logger);
    }

    [Fact]
    public async Task Handle_OrderInCache_ReturnsCachedOrder()
    {
        var orderId = Guid.NewGuid();
        var cachedOrder = Order.Restore(orderId, "Cliente", 100m, DateTime.UtcNow);
        _cache.GetAsync(orderId).Returns(cachedOrder);

        var result = await _handler.Handle(new GetOrderQuery(orderId), CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(orderId);
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderNotInCache_QueriesDatabase()
    {
        var orderId = Guid.NewGuid();
        var order = Order.Restore(orderId, "Cliente", 100m, DateTime.UtcNow);
        _cache.GetAsync(orderId).Returns((Order?)null);
        _repository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handler.Handle(new GetOrderQuery(orderId), CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(orderId);
        await _repository.Received(1).GetByIdAsync(orderId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderNotInCacheNorDatabase_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        _cache.GetAsync(orderId).Returns((Order?)null);
        _repository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _handler.Handle(new GetOrderQuery(orderId), CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorOr.ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_OrderFoundInDatabase_SavesToCache()
    {
        var orderId = Guid.NewGuid();
        var order = Order.Restore(orderId, "Cliente", 100m, DateTime.UtcNow);
        _cache.GetAsync(orderId).Returns((Order?)null);
        _repository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns(order);

        await _handler.Handle(new GetOrderQuery(orderId), CancellationToken.None);

        await _cache.Received(1).SetAsync(order);
    }
}
