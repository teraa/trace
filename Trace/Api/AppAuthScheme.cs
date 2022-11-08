using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Trace.Api;

public static class AppAuthScheme
{
    public const string Bearer = JwtBearerDefaults.AuthenticationScheme;
    public const string ExpiredBearer = nameof(ExpiredBearer);
}
