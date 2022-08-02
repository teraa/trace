namespace Trace.Api.Extensions;

public static class UriBuilderExtensions
{
    public static UriBuilder AddQuery(this UriBuilder uriBuilder, IReadOnlyDictionary<string, string?> query)
    {
        var pairs = query.Select(x => x.Value is null
            ? $"{Uri.EscapeDataString(x.Key)}"
            : $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}");

        uriBuilder.Query = string.Join('&', pairs);

        return uriBuilder;
    }
}
