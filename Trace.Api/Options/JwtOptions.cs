// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618
namespace Trace.Api.Options;

public class JwtOptions
{
    public string SigningKey { get; init; }
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public TimeSpan TokenLifetime { get; init; }
    public TimeSpan RefreshTokenLifetime { get; init; }
    public TimeSpan ClockSkew { get; init; }
}
