using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Tests;

public class Test1 : IClassFixture<AppFactory>
{
    private readonly AppFactory _appFactory;

    public Test1(AppFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task TestAsync()
    {
        using var scope = _appFactory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ctx.TwitchUsers.Add(new User
        {
            EntryId = 0,
            Login = "example",
        });

        await ctx.SaveChangesAsync();

        var count = await ctx.TwitchUsers.CountAsync();
        count.Should().Be(1);

        await _appFactory.ResetDatabaseAsync();

        count = await ctx.TwitchUsers.CountAsync();
        count.Should().Be(0);
    }
}
