using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;

namespace Trace.Api.Features.Auth;

public static class MemoryCacheExtensions
{
    public static string CreateState(this IMemoryCache cache, TimeSpan lifetime)
    {
        var state = Guid.NewGuid().ToString();
        cache.Set<object?>(new StateKey(state), null, lifetime);
        return state;
    }

    public static bool InvalidateState(this IMemoryCache cache, string state)
    {
        var key = new StateKey(state);
        bool result = cache.TryGetValue(key, out _);
        cache.Remove(key);

        return result;
    }


    [UsedImplicitly]
    private record StateKey(string State);
}
