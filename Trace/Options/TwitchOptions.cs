using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class TwitchOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public Uri RedirectUri { get; init; }
    public string Scope { get; set; } = "";

    [UsedImplicitly]
    public class Validator : AbstractValidator<TwitchOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.RedirectUri).NotEmpty();
            RuleFor(x => x.Scope).NotNull();
        }
    }
}
