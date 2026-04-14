using FluentValidation;

namespace OrderProcessing.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Client)
            .NotEmpty().WithMessage("Client name is required.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Order value must be greater than zero.");
    }
}
