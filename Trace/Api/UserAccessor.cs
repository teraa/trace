using System.Security.Claims;

namespace Trace.Api;

public interface IUserAccessor
{
    ClaimsPrincipal User { get; }
}

public sealed class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User
        => _httpContextAccessor.HttpContext?.User
           ?? throw new InvalidOperationException($"{nameof(_httpContextAccessor.HttpContext)} was not set.");
}
