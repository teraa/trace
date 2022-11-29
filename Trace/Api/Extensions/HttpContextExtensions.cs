using System.Diagnostics;
using System.Security.Claims;

namespace Trace.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var value = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Debug.Assert(value is { });
        return new Guid(value);
    }
}
