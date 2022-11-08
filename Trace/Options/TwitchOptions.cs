using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class TwitchOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public TimeSpan StateLifetime { get; init; } = TimeSpan.FromMinutes(5);
    public Uri AuthorizationEndpoint { get; init; } = new("https://id.twitch.tv/oauth2/authorize");
    public Uri RedirectUri { get; init; }
    public Uri TokenEndpoint { get; init; } = new("https://id.twitch.tv/oauth2/token");
    public Uri ValidateEndpoint { get; init; } = new("https://id.twitch.tv/oauth2/validate");
    public string Scope { get; set; } = "";

    [UsedImplicitly]
    public class Validator : AbstractValidator<TwitchOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.StateLifetime).GreaterThan(TimeSpan.Zero);
            RuleFor(x => x.AuthorizationEndpoint).NotEmpty();
            RuleFor(x => x.RedirectUri).NotEmpty();
            RuleFor(x => x.TokenEndpoint).NotEmpty();
            RuleFor(x => x.ValidateEndpoint).NotEmpty();
            RuleFor(x => x.Scope).NotNull();
        }
    }
}
