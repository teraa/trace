using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Trace.Api;

[UsedImplicitly]
public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException ex)
            return;

        foreach (var error in ex.Errors)
            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        var problemDetails = new ValidationProblemDetails(context.ModelState);

        context.Result = new BadRequestObjectResult(problemDetails);
    }
}
