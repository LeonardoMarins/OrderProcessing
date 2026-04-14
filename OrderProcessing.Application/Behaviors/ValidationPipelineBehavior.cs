using ErrorOr;
using FluentValidation;
using MediatR;

namespace OrderProcessing.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var errors = _validators
            .SelectMany(v => v.Validate(request).Errors)
            .Where(f => f != null)
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
        {
            var errorOrResult = (TResponse)typeof(ErrorOrFactory)
                .GetMethod(nameof(ErrorOrFactory.From))!
                .MakeGenericMethod(typeof(TResponse).GetGenericArguments()[0])
                .Invoke(null, [errors])!;

            return errorOrResult;
        }

        return await next();
    }
}
