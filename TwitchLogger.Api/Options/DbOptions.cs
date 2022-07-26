using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace TwitchLogger.Api.Options;

[UsedImplicitly]
public class DbOptions
{
    public string ConnectionString { get; init; }
}
