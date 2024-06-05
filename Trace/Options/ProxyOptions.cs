using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Teraa.Extensions.Configuration;

namespace Trace.Options;

#pragma warning disable CS8618
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ProxyOptions
{
    public ForwardedHeaders ForwardedHeaders { get; init; } = ForwardedHeaders.None;
    public int? ForwardLimit { get; init; }
    public IReadOnlyList<string> KnownNetworks { get; init; } = [];

    public sealed class Validator : AbstractValidator<ProxyOptions>
    {
        public Validator()
        {
            RuleForEach(x => x.KnownNetworks)
                .Must(x => IPNetwork.TryParse(x, out _))
                .WithMessage("Value must be a valid IPNetwork");
        }
    }
}

public static class ProxyExtensions
{
    public static IApplicationBuilder UseForwardedHeaders(this IApplicationBuilder builder, IConfiguration configuration)
    {
        var options = configuration.GetValidatedOptions([new ProxyOptions.Validator()]);

        if (options is null || options.ForwardedHeaders is ForwardedHeaders.None)
            return builder;

        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = options.ForwardedHeaders,
            ForwardLimit = options.ForwardLimit,
        };

        foreach (var network in options.KnownNetworks)
        {
            forwardedHeadersOptions.KnownNetworks.Add(IPNetwork.Parse(network));
        }

        builder.UseForwardedHeaders(forwardedHeadersOptions);

        return builder;
    }
}
