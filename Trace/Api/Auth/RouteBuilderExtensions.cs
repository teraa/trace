using System.Security.Claims;
using AspNet.Security.OAuth.Twitch;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Trace.Data;

namespace Trace.Api.Auth;

public static class RouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth");

        group.MapGet("/login", async (SignInManager<AppUser> signInManager, HttpContext httpContext) =>
        {
            // Clear the existing external cookie (set by /api/signin-twitch) to ensure a clean login process
            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var properties = signInManager.ConfigureExternalAuthenticationProperties(
                "Twitch",
                // need to specify auth because this is executed from /api/signing-twitch,
                // and we want to redirect to /api/auth/continue
                "auth/continue"
            );

            return TypedResults.Challenge(
                properties,
                [TwitchAuthenticationDefaults.AuthenticationScheme]
            );
        });


        group.MapGet("/continue", async Task<
            Results<
                RedirectHttpResult,
                BadRequest,
                BadRequest<SignInResult>,
                ForbidHttpResult
            >
        > (
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IOptionsSnapshot<TwitchAuthOptions> options,
            ILoggerFactory loggerFactory,
            ClaimsPrincipal claimsPrincipal
        ) =>
        {
            var logger = loggerFactory.CreateLogger(typeof(RouteBuilderExtensions).FullName!); // TODO
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                logger.LogWarning("Couldn't load external login information");
                return TypedResults.BadRequest();
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true,
                bypassTwoFactor: true);

            // Existing user
            if (signInResult.Succeeded)
            {
                logger.LogInformation("User {ProviderKey} logged in with {LoginProvider}", info.ProviderKey,
                    info.LoginProvider);
                return TypedResults.Redirect(options.Value.RedirectUri.ToString());
            }

            if (signInResult.IsLockedOut)
            {
                return TypedResults.Forbid();
            }

            // New user
            var user = new AppUser();
            var twitchLogin = claimsPrincipal.Identity!.Name!;
            var twitchId = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            await userManager.SetUserNameAsync(user, $"{twitchLogin}:{twitchId.Value}"); // Whatever, not important

            var identityResult = await userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                logger.LogWarning("Failed to create user: {Errors}", identityResult.Errors.Select(x => x.Description));
                return TypedResults.BadRequest();
            }

            identityResult = await userManager.AddClaimsAsync(user,
            [
                new Claim(AppClaimTypes.TwitchId, twitchId.Value),
                new Claim(AppClaimTypes.TwitchLogin, twitchLogin)
            ]);

            if (!identityResult.Succeeded)
            {
                logger.LogWarning("Failed to add user claims: {Errors}",
                    identityResult.Errors.Select(x => x.Description));
                return TypedResults.BadRequest();
            }

            identityResult = await userManager.AddLoginAsync(user, info);
            if (!identityResult.Succeeded)
            {
                logger.LogWarning("Failed to add login for user: {Errors}",
                    identityResult.Errors.Select(x => x.Description));
                return TypedResults.BadRequest();
            }

            signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                logger.LogInformation("User {ProviderKey} created an account with {LoginProvider}", info.ProviderKey,
                    info.LoginProvider);
                return TypedResults.Redirect(options.Value.RedirectUri.ToString());
            }

            return TypedResults.BadRequest(signInResult);
        }).RequireAuthorization(
            new AuthorizeAttribute
            {
                // Both Identity.External and Twitch schemes will work here,
                // but Twitch will redirect to twitch.tv and External will redirect to /Account/Login...
                // when request is unauthenticated
                AuthenticationSchemes = TwitchAuthenticationDefaults.AuthenticationScheme,
            }
        );


        group.MapPost(
            "/logout",
            // This just deletes the cookie client side, but it's still valid
            () => TypedResults.SignOut(authenticationSchemes: [IdentityConstants.ApplicationScheme])
        );


        // TODO: Disable in production
        group.MapGet(
            "/info",
            (ClaimsPrincipal claimsPrincipal) => TypedResults.Ok(
                claimsPrincipal.Identities.Select(i => new
                {
                    i.AuthenticationType,
                    Claims = i.Claims.Select(c => new
                    {
                        c.Type,
                        c.Value,
                    }),
                })
            )
        ).RequireAuthorization();

        return group;
    }
}
