using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Trace.Data;
using Trace.Data.Models.Twitch;
using Index = Trace.Api.Twitch.Users.Actions.Index;

namespace Trace.Tests.Twitch.Users;

[Collection("1")]
public sealed class IndexTests : IAsyncLifetime, IDisposable
{
    private readonly AppFactory _appFactory;
    private readonly IServiceScope _scope;
    private readonly Index.Handler _handler;
#pragma warning disable CA2213
    private readonly AppDbContext _ctx;
#pragma warning restore CA2213

    public IndexTests(AppFactory appFactory)
    {
        _appFactory = appFactory;
        _scope = _appFactory.Services.CreateScope();
        _handler = _scope.ServiceProvider.GetRequiredService<Index.Handler>();
        _ctx = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private User Template { get; } = new()
    {
        FirstSeen = DateTimeOffset.MinValue,
        LastSeen = DateTimeOffset.MinValue,
    };

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _appFactory.ResetDatabaseAsync();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task QueryWithIds_ReturnsCorrectUsers()
    {
        var users = new List<User>
        {
            Template with {Id = "1", Login = "foo"},
            Template with {Id = "2", Login = "bar"}, // except
            Template with {Id = "1", Login = "baz"},
            Template with {Id = "3", Login = "qux"},
        };

        _ctx.TwitchUsers.AddRange(users);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(Ids: ["1", "3"]);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new Index.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithLogins_ReturnsCorrectUsers()
    {
        var users = new List<User>
        {
            Template with {Id = "1", Login = "foo"},
            Template with {Id = "2", Login = "bar"}, // except
            Template with {Id = "3", Login = "foo"},
            Template with {Id = "4", Login = "qux"},
        };

        _ctx.TwitchUsers.AddRange(users);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(Logins: ["foo", "qux"]);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new Index.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithLoginPattern_ReturnsCorrectUsers()
    {
        var users = new List<User>
        {
            Template with {Id = "1", Login = "foo"}, // except
            Template with {Id = "2", Login = "bar"},
            Template with {Id = "3", Login = "baz"},
        };

        _ctx.TwitchUsers.AddRange(users);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(LoginPattern: "^BA");


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Should().HaveCount(2)
            .And.BeEquivalentTo(
                users.Skip(1)
                    .Select(x => new Index.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithRecursiveLogins_ReturnsCorrectUsers()
    {
        var users = new List<User>
        {
            Template with {Id = "1", Login = "foo"},
            Template with {Id = "2", Login = "bar"}, // except
            Template with {Id = "3", Login = "foo"},
            Template with {Id = "3", Login = "qux"},
        };

        _ctx.TwitchUsers.AddRange(users);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(Logins: ["foo"], Recursive: true);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new Index.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }
}

file static class Extensions
{
    public static AndWhichConstraint<ObjectAssertions, List<Index.Result>> BeResults(
        this ObjectAssertions assertions)
    {
        return assertions.BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<List<Index.Result>>();
    }
}
