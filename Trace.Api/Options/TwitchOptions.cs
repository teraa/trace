// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace Trace.Api.Options;

public class TwitchOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }

    public TimeSpan StateLifetime { get; init; }
    public Uri AuthorizationEndpoint { get; init; }
    public Uri RedirectUri { get; init; }
    public string Scope { get; set; }
    public Uri TokenEndpoint { get; init; }
    public Uri ValidateEndpoint { get; init; }
}
