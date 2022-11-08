using System.Security.Claims;

namespace Trace.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        string value = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return new Guid(value);
    }
}
