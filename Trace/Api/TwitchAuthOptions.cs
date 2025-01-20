using FluentValidation;
using JetBrains.Annotations;

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
