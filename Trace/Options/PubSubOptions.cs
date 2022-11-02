using FluentValidation;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.Options;

#pragma warning disable CS8618
public class PubSubOptions
{
    public string Token { get; init; }

    [UsedImplicitly]
    public class Validator : AbstractValidator<PubSubOptions>
    {
        public Validator()
        {
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}
