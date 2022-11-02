using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class TmiOptions
{
    public string MessageSourceName { get; init; }
    public string Login { get; init; }
    public string Token { get; init; }

    [UsedImplicitly]
    public class Validator : AbstractValidator<TmiOptions>
    {
        public Validator()
        {
            RuleFor(x => x.MessageSourceName).NotEmpty();
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}
