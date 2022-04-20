using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TwitchLogger.Data;

namespace TwitchLogger;

public static class Extensions
{
    public static TService Get<TService>(this IServiceScope scope)
        where TService : notnull
        => scope.ServiceProvider.GetRequiredService<TService>();

    private static IConfigurationSection GetOptionsSection<TOptions>(this IConfiguration configuration)
    {
        const string suffix = "Options";

        string name = typeof(TOptions).Name;

        if (name.EndsWith(suffix))
            name = name[..^suffix.Length];

        return configuration.GetRequiredSection(name);
    }

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
    {
        return configuration.GetOptionsSection<TOptions>().Get<TOptions>();
    }

    public static IServiceCollection AddOptionsWithSection<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        return services.Configure<TOptions>(configuration.GetOptionsSection<TOptions>());
    }

    public static async Task<bool> TryUpdateUserAsync(
        this TwitchLoggerDbContext ctx,
        string id,
        string login,
        DateTimeOffset timestamp,
        IMemoryCache cache,
        MemoryCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var key = $"user.{id}";

        if (cache.TryGetValue(key, out string cachedLogin) && cachedLogin == login)
            return false;

        var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
        {
            user = new Data.Models.Twitch.User
            {
                Id = id,
                Login = login,
                FirstSeenAt = timestamp,
            };

            ctx.Users.Add(user);
        }
        else if (user.Login != login)
        {
            user.Login = login;
        }

        cache.Set(key, login, options);

        return true;
    }
}
