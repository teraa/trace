using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class TwitchOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public Uri RedirectUri { get; init; } = new("/", UriKind.Relative);

    [UsedImplicitly]
    public class Validator : AbstractValidator<TwitchOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.RedirectUri).NotEmpty();
        }
    }
}
