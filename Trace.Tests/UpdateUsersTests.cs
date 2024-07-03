using System.Globalization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trace.Data;
using Handler = Trace.Features.Users.UpdateUsers.Handler;
using Command = Trace.Features.Users.UpdateUsers.Command;
using User = Trace.Features.Users.UpdateUsers.Command.User;

namespace Trace.Tests;

public sealed class UpdateUsersTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _appFactory;

    public UpdateUsersTests(AppFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task OneUser_InsertsOne()
    {
        using var scope = _appFactory.Services.CreateScope();
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
        using var scope = _appFactory.Services.CreateScope();
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
