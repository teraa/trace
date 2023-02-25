using FluentValidation;
using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace Trace.Options;

[UsedImplicitly]
public class VaultOptions
{
    public bool IsEnabled { get; init; } = false;
    public Uri Address { get; init; } = new("http://localhost:8200");
    public string Token { get; init; }
    public string Mount { get; init; } = "secret";
    public string Path { get; init; } = "Trace";

    public class Validator : AbstractValidator<VaultOptions>
    {
        public Validator()
        {
            When(x => x.IsEnabled, () =>
            {
                RuleFor(x => x.Address).NotEmpty();
                RuleFor(x => x.Token).NotEmpty();
                RuleFor(x => x.Mount).NotEmpty();
                RuleFor(x => x.Path).NotEmpty();
            });
        }
    }
}
