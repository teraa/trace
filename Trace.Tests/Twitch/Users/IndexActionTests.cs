using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Trace.Api.Twitch.Users;
using Trace.Data.Models.Twitch;

namespace Trace.Tests.Twitch.Users;

public sealed class IndexActionTests(AppFactory appFactory) : AppTests(appFactory)
{
    private User UserTemplate { get; } = new()
    {
        FirstSeen = DateTimeOffset.MinValue,
        LastSeen = DateTimeOffset.MinValue,
    };


    [Fact]
    public async Task QueryWithIds_ReturnsCorrectUsers()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();

        var users = new List<User>
        {
            UserTemplate with {Id = "1", Login = "foo"},
            UserTemplate with {Id = "2", Login = "bar"}, // except
            UserTemplate with {Id = "1", Login = "baz"},
            UserTemplate with {Id = "3", Login = "qux"},
        };

        ctx.TwitchUsers.AddRange(users);
        await ctx.SaveChangesAsync();

        var query = new IndexAction.Query(Ids: ["1", "3"]);


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithLogins_ReturnsCorrectUsers()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();

        var users = new List<User>
        {
            UserTemplate with {Id = "1", Login = "foo"},
            UserTemplate with {Id = "2", Login = "bar"}, // except
            UserTemplate with {Id = "3", Login = "foo"},
            UserTemplate with {Id = "4", Login = "qux"},
        };

        ctx.TwitchUsers.AddRange(users);
        await ctx.SaveChangesAsync();

        var query = new IndexAction.Query(Logins: ["foo", "qux"]);


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithLoginPattern_ReturnsCorrectUsers()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();

        var users = new List<User>
        {
            UserTemplate with {Id = "1", Login = "foo"}, // except
            UserTemplate with {Id = "2", Login = "bar"},
            UserTemplate with {Id = "3", Login = "baz"},
        };

        ctx.TwitchUsers.AddRange(users);
        await ctx.SaveChangesAsync();

        var query = new IndexAction.Query(LoginPattern: "^BA");


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(2)
            .And.BeEquivalentTo(
                users.Skip(1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithRecursiveLogins_ReturnsCorrectUsers()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();

        var users = new List<User>
        {
            UserTemplate with {Id = "1", Login = "foo"},
            UserTemplate with {Id = "2", Login = "bar"}, // except
            UserTemplate with {Id = "3", Login = "foo"},
            UserTemplate with {Id = "3", Login = "qux"},
        };

        ctx.TwitchUsers.AddRange(users);
        await ctx.SaveChangesAsync();

        var query = new IndexAction.Query(Logins: ["foo"], Recursive: true);


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithRecursiveLoginPattern_ReturnsCorrectUsers()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();

        var users = new List<User>
        {
            UserTemplate with {Id = "1", Login = "foo"},
            UserTemplate with {Id = "2", Login = "bar"}, // except
            UserTemplate with {Id = "3", Login = "foo"},
            UserTemplate with {Id = "3", Login = "qux"},
        };

        ctx.TwitchUsers.AddRange(users);
        await ctx.SaveChangesAsync();

        var query = new IndexAction.Query(LoginPattern: "^f", Recursive: true);


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResults()
            .Subject.Value.Should().HaveCount(3)
            .And.BeEquivalentTo(
                users.Where((_, i) => i != 1)
                    .Select(x => new IndexAction.Result(x.Id, x.Login, x.FirstSeen, x.LastSeen)));
    }

    [Fact]
    public async Task QueryWithInvalidPattern_ReturnsBadRequest()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var query = new IndexAction.Query(LoginPattern: "[z-a]");


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOfType<ProblemHttpResult>().Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task QueryWithRecursiveInvalidPattern_ReturnsBadRequest()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var query = new IndexAction.Query(LoginPattern: "[z-a]", Recursive: true);


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
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
