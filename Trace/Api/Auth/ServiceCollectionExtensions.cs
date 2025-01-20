using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Teraa.Shared.Configuration;
using Trace.Data;

namespace Trace.Api.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters += ":";
                // options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            // .AddUserConfirmation<>()
            ;

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureApplicationCookie(options => { options.Cookie.Name = "Auth"; })
            .ConfigureExternalCookie(options => { options.Cookie.Name = "External"; })
            .AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CustomAuthenticationHandler.SchemeName;
                options.DefaultForbidScheme = CustomAuthenticationHandler.SchemeName;
                options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationHandler.SchemeName, null);
            })
            .AddTwitchAuth(configuration)
            .Services
            .AddAuthorization(options =>
            {
                options.AddPolicy(AppAuthzPolicies.Channel,
                    policy => { policy.Requirements.Add(new ChannelAuthorizationHandler.Requirement()); });
            })
            .AddSingleton<IAuthorizationHandler, ChannelAuthorizationHandler>()
            ;


        return services;
    }

    private static AuthenticationBuilder AddTwitchAuth(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        builder
            .AddTwitch(authOptions =>
            {
                var twitchOptions = configuration.GetValidatedRequiredOptions([new TwitchAuthOptions.Validator()], "Twitch");
                authOptions.ClientId = twitchOptions.ClientId;
                authOptions.ClientSecret = twitchOptions.ClientSecret;
                authOptions.CallbackPath = twitchOptions.CallbackPath;

                authOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                authOptions.CorrelationCookie.Name = "Correlation.";

                // options.SaveTokens = true;
            })
            .Services
            .AddValidatedOptions<TwitchAuthOptions>(configSectionPath: "Twitch")
            ;

        return builder;
    }
}
