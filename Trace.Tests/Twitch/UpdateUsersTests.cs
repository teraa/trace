using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Command = Trace.Common.UpdateUsers.Command;
using Handler = Trace.Common.UpdateUsers.Handler;
using User = Trace.Common.UpdateUsers.Command.User;

namespace Trace.Tests.Twitch;

public sealed class UpdateUsersTests(AppFactory appFactory) : AppTests(appFactory)
{
    [Fact]
    public async Task OneUser_InsertsOne()
    {
        using var scope = CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Handler>();
        var time = DateTimeOffset.Parse("2000-01-01T00:00:00Z", CultureInfo.InvariantCulture);
        var command = new Command([new User("10", "foo")], time);


        await handler.HandleAsync(command);


        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = await ctx.TwitchUsers.ToListAsync();
        users.Should().HaveCount(1);

        var user = users[0];
        user.Id.Should().Be("10");
        user.Login.Should().Be("foo");
        user.LastSeen.Should().Be(time);
        user.FirstSeen.Should().Be(time);
    }

    [Fact]
    public async Task TwoUsersSameId_InsertsOne()
    {
        using var scope = CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Handler>();
        var time = DateTimeOffset.Parse("2000-01-01T00:00:00Z", CultureInfo.InvariantCulture);
        var command = new Command([
            new User("10", "foo"),
            new User("10", "bar"),
        ], time);


        await handler.HandleAsync(command);


        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = await ctx.TwitchUsers.ToListAsync();
        users.Should().HaveCount(1);

        var user = users[0];
        user.Id.Should().Be("10");
        user.Login.Should().Be("foo");
        user.LastSeen.Should().Be(time);
        user.FirstSeen.Should().Be(time);
    }
}
