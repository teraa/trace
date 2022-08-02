using JetBrains.Annotations;

namespace Trace.Api.Extensions;

public static class ConfigurationExtensions
{
    public static IConfigurationSection GetOptionsSection<TOptions>(this IConfiguration configuration)
    {
        const string suffix = "Options";

        string name = typeof(TOptions).Name;

        if (name.EndsWith(suffix))
            name = name[..^suffix.Length];

        return configuration.GetRequiredSection(name);
    }

    public static TOptions GetOptions<[MeansImplicitUse] TOptions>(this IConfiguration configuration)
    {
        return configuration.GetOptionsSection<TOptions>().Get<TOptions>();
    }

    public static IServiceCollection AddOptionsWithSection<[MeansImplicitUse] TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        return services.Configure<TOptions>(configuration.GetOptionsSection<TOptions>());
    }
}
