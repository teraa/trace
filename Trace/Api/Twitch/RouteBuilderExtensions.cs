using Microsoft.AspNetCore.Mvc;

namespace Trace.Api.Twitch;

public static class RouteBuilderExtensions
{
    public static RouteGroupBuilder MapTwitch(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/twitch")
            .RequireAuthorization();

        group.MapChannels();
        group.MapMessages();
        group.MapUsers();

        return group;
    }

    private static RouteGroupBuilder MapChannels(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/channels");

        group.MapGet(
            "",
            async (Channels.IndexAction.Handler handler, CancellationToken cancellationToken)
                => await handler.HandleAsync(new Channels.IndexAction.Query(), cancellationToken)
        );

        return group;
    }

    private static RouteGroupBuilder MapMessages(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/messages");

        group.MapGet(
            "",
            async ([AsParameters] Messages.IndexAction.Query query, Messages.IndexAction.Handler handler,
                    CancellationToken cancellationToken)
                => await handler.HandleAsync(query, cancellationToken)
        );

        return group;
    }

    private static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .AllowAnonymous() // TODO: disable
            ;

        group.MapPost(
            "",
            async ([FromBody] Users.IndexAction.Query query, [FromServices] Users.IndexAction.Handler handler,
                    CancellationToken cancellationToken)
                => await handler.HandleAsync(query, cancellationToken)
        );

        return group;
    }
}
