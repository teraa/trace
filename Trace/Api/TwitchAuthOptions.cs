using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Teraa.Extensions.Configuration;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Api;

#pragma warning disable CS8618
public class TwitchAuthOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public Uri RedirectUri { get; init; } = new("/", UriKind.Relative);
    public PathString CallbackPath { get; init; } = "/api/signin-twitch";

    [UsedImplicitly]
    public class Validator : AbstractValidator<TwitchAuthOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.RedirectUri).NotEmpty();
            RuleFor(x => x.CallbackPath).NotEmpty();
        }
    }
}

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddTwitchAuth(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        builder
            .AddTwitch(authOptions =>
            {
                var twitchOptions = configuration.GetValidatedRequiredOptions([new TwitchAuthOptions.Validator()]);
                authOptions.ClientId = twitchOptions.ClientId;
                authOptions.ClientSecret = twitchOptions.ClientSecret;
                authOptions.CallbackPath = twitchOptions.CallbackPath;

                authOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                authOptions.CorrelationCookie.Name = "Correlation.";

                // options.SaveTokens = true;
            })
            .Services
            .AddValidatedOptions<TwitchAuthOptions>()
            ;

        return builder;
    }
}
