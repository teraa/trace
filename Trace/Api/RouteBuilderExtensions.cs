using Trace.Api.Auth;
using Trace.Api.Twitch;

namespace Trace.Api;

public static class RouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api");

        group.MapAuth();
        group.MapTwitch();

        return group;
    }
}
