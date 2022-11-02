// ReSharper disable UnusedAutoPropertyAccessor.Global

using FluentValidation;
using JetBrains.Annotations;

namespace Trace.Api.Options;

#pragma warning disable CS8618
public class TwitchOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public TimeSpan StateLifetime { get; init; }
    public Uri AuthorizationEndpoint { get; init; }
    public Uri RedirectUri { get; init; }
    public Uri TokenEndpoint { get; init; }
    public Uri ValidateEndpoint { get; init; }
    public string Scope { get; set; }

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
