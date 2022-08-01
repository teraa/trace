using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Trace.Api;

public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, IActionResult>
    where TRequest : IRequest<TResponse>, IRequest<IActionResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<IActionResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<IActionResult> next)
    {
        if (!_validators.Any())
            return next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .ToList();

        if (!failures.Any())
            return next();

        var errors = failures.GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());

        var problemDetails = new ValidationProblemDetails(errors);
        IActionResult result = new BadRequestObjectResult(problemDetails);

        return Task.FromResult(result);
    }
}
