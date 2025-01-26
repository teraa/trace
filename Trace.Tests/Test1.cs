using Microsoft.EntityFrameworkCore;
using Trace.Data.Models.Twitch;

namespace Trace.Tests;

public class Test1(AppFactory appFactory) : AppTests(appFactory)
{
    [Fact]
    public async Task TestAsync()
    {
        using var scope = CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ctx.TwitchUsers.Add(new User
        {
            EntryId = 0,
            Login = "example",
        });

        await ctx.SaveChangesAsync();

        var count = await ctx.TwitchUsers.CountAsync();
        count.Should().Be(1);

        await AppFactory.ResetDatabaseAsync();

        count = await ctx.TwitchUsers.CountAsync();
        count.Should().Be(0);
    }
}
