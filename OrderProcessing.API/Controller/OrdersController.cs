using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OrderProcessing.Controller;

public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [Route("orders")]
    public IActionResult CreateOrder()
    {
        // Logic to create an order and send it to the message queue
        return Ok("Order created and sent to the message queue.");
    }
    
    [HttpGet]
    [Route("orders/{id:guid}")]
    public IActionResult GetOrder(Guid id)
    {
        // Logic to create an order and send it to the message queue
        return Ok("Order created and sent to the message queue.");
    }
    
    [HttpGet]
    [Route("orders")]
    public IActionResult GetOrders()
    {
        // Logic to create an order and send it to the message queue
        return Ok("Order created and sent to the message queue.");
    }
}