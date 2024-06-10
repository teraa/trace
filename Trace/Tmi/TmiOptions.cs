using FluentValidation;
using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Tmi;

#pragma warning disable CS8618
public class TmiOptions
{
    public string MessageSourceName { get; init; } = "Trace";
    public string Login { get; init; } = "justinfan1";
    public string Token { get; init; } = "x";

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
