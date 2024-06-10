using FluentValidation;
using JetBrains.Annotations;
using Teraa.Extensions.Configuration;
using Teraa.Twitch.PubSub;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Trace.PubSub;

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

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPubSub(this IServiceCollection services)
    {
        services
            .AddValidatedOptions<PubSubOptions>()
            .AddPubSubService();

        return services;
    }
}
