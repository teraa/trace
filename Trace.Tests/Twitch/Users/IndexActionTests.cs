using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api.Twitch.Users;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Tests.Twitch.Users;

public sealed class IndexActionTests : AppTests, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IndexAction.Handler _handler;
#pragma warning disable CA2213
    private readonly AppDbContext _ctx;
#pragma warning restore CA2213

    public IndexActionTests(AppFactory appFactory) : base(appFactory)
    {
        _scope = CreateScope();
        _handler = _scope.ServiceProvider.GetRequiredService<IndexAction.Handler>();
        _ctx = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private User Template { get; } = new()
    {
        FirstSeen = DateTimeOffset.MinValue,
        LastSeen = DateTimeOffset.MinValue,
    };

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

        var query = new IndexAction.Query(Ids: ["1", "3"]);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
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

        var query = new IndexAction.Query(Logins: ["foo", "qux"]);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
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

        var query = new IndexAction.Query(LoginPattern: "^BA");


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(2)
            .And.BeEquivalentTo(
                users.Skip(1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
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

        var query = new IndexAction.Query(Logins: ["foo"], Recursive: true);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithRecursiveLoginPattern_ReturnsCorrectUsers()
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

        var query = new IndexAction.Query(LoginPattern: "^f", Recursive: true);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithInvalidPattern_ReturnsBadRequest()
    {
        var query = new IndexAction.Query(LoginPattern: "[z-a]");


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOfType<ProblemHttpResult>().Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task QueryWithRecursiveInvalidPattern_ReturnsBadRequest()
    {
        var query = new IndexAction.Query(LoginPattern: "[z-a]", Recursive: true);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOfType<ProblemHttpResult>().Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}

file static class Extensions
{
    public static AndWhichConstraint<ObjectAssertions, Ok<List<IndexAction.Result>>> BeOkResults(
        this ObjectAssertions assertions)
    {
        return assertions.BeOfType<Ok<List<IndexAction.Result>>>();
    }
}
