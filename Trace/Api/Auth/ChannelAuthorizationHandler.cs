using Microsoft.AspNetCore.Authorization;

namespace Trace.Api.Auth;

public sealed class ChannelAuthorizationHandler : AuthorizationHandler<ChannelAuthorizationHandler.Requirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement, string resource)
    {
        if (context.User.HasClaim(AppClaimTypes.ChannelRead, resource))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Requirement : IAuthorizationRequirement;
}
