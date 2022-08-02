using Microsoft.AspNetCore.Mvc;

namespace Trace.Api;

public static class Results
{
    public static BadRequestObjectResult BadRequestDetails(string detail)
        => new(new ProblemDetails
        {
            Title = detail,
        });
}