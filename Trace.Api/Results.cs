using Microsoft.AspNetCore.Mvc;

namespace Trace.Api;

public static class Results
{
    public static BadRequestObjectResult BadRequestDetails(string title)
        => new(new ProblemDetails
        {
            Title = title,
        });
}
