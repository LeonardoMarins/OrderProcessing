using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Orders.Commands.CreateOrder;
using OrderProcessing.Application.Orders.Queries.GetOrder;
using OrderProcessing.Application.Orders.Queries.GetOrders;

namespace OrderProcessing.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError)
            return Problem(result.FirstError.Description, statusCode: 400);

        _logger.LogInformation("Order created successfully with Id {OrderId}", result.Value);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, null);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderQuery(id), cancellationToken);

        if (result.IsError)
            return result.FirstError.Type == ErrorOr.ErrorType.NotFound
                ? NotFound()
                : Problem(result.FirstError.Description, statusCode: 500);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrdersQuery(), cancellationToken);
        return Ok(orders);
    }
}
