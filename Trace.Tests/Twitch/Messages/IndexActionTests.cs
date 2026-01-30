using System.Security.Claims;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Trace.Api.Auth;
using Trace.Api.Twitch.Messages;
using Trace.Data.Models.Tmi;

namespace Trace.Tests.Twitch.Messages;

public sealed class IndexActionTests(AppFactory appFactory) : AppTests(appFactory)
{
    private const string s_channelId = "channel.id";

    private Message MessageTemplate { get; } = new()
    {
        AuthorId = "author.id",
        AuthorLogin = "author.login",
        ChannelId = s_channelId,
        Content = "Lorem ipsum",
        Source = new Source { Name = "source" },
        Timestamp = DateTimeOffset.MinValue,
    };

    private IndexAction.Query QueryTemplate { get; } = new(s_channelId);

    private void SetupUser()
    {
        AppFactory.UserAccessorMock.Setup(x => x.User)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim(AppClaimTypes.ChannelRead, s_channelId)])));
    }


    [Fact]
    public async Task Forbidden_WhenUnauthorized()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        SetupUser();

        var request = new IndexAction.Query("foo");


        // Act
        var actionResult = await handler.HandleAsync(request);


        // Assert
        actionResult.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public async Task Returns_CorrectChannelMessages()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();
        SetupUser();

        var messages = new List<Message>
        {
            MessageTemplate with {Id = 1},
            MessageTemplate with {Id = 2, ChannelId = "foo"},
            MessageTemplate with {Id = 3, ChannelId = "bar"},
            MessageTemplate with {Id = 4},
            MessageTemplate with {Id = 5, ChannelId = "foo"},
            MessageTemplate with {Id = 6, ChannelId = "bar"},
        };

        ctx.TmiMessages.AddRange(messages);
        await ctx.SaveChangesAsync();


        // Act
        var response = await handler.HandleAsync(QueryTemplate);


        // Assert
        response.Should().BeOkResult().Subject.Value!
            .Select(x => x.Id)
            .Should().BeEquivalentTo([1, 4]);
    }

    [Fact]
    public async Task QueryWithBefore_RespectsTimestampNotId()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();
        SetupUser();

        var start = DateTimeOffset.MinValue;
        var second = new TimeSpan(0, 0, 1);

        var messages = new List<Message>
        {
            MessageTemplate with {Id = 1, Timestamp = start + second * 1}, // before
            MessageTemplate with {Id = 5, Timestamp = start + second * 2}, // before
            MessageTemplate with {Id = 3, Timestamp = start + second * 3}, // before
            MessageTemplate with {Id = 2, Timestamp = start + second * 4}, // pivot
            MessageTemplate with {Id = 4, Timestamp = start + second * 5}, // after
        };

        ctx.TmiMessages.AddRange(messages);
        await ctx.SaveChangesAsync();

        var query = QueryTemplate with { Before = 2 };


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResult().Subject.Value!
            .Select(x => x.Id)
            .Should().Equal([3, 5, 1]);
    }

    [Fact]
    public async Task QueryWithInvalidBefore_ReturnsBadRequest()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();
        SetupUser();

        var messages = new List<Message>
        {
            MessageTemplate with {Id = 1},
            MessageTemplate with {Id = 2, ChannelId = "foo"},
        };

        ctx.TmiMessages.AddRange(messages);
        await ctx.SaveChangesAsync();

        var query = QueryTemplate with { Before = 2 };


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOfType<ProblemHttpResult>().Subject
            .StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task QueryWithAuthorId_ReturnsOnlyMessagesByAuthorId()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();
        SetupUser();

        var messages = new List<Message>
        {
            MessageTemplate with {Id = 1, AuthorId = "1"}, // target
            MessageTemplate with {Id = 2, AuthorId = "2"},
            MessageTemplate with {Id = 3, AuthorId = "3"},
            MessageTemplate with {Id = 4, AuthorId = "1"}, // target
            MessageTemplate with {Id = 5, AuthorId = "5"},
        };

        ctx.TmiMessages.AddRange(messages);
        await ctx.SaveChangesAsync();

        const string authorId = "1";
        var query = QueryTemplate with { AuthorId = authorId };


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        var results = response.Should().BeOkResult().Subject.Value!;

        results.Select(x => x.AuthorId)
            .Should().AllBe(authorId);

        results.Select(x => x.Id)
            .Should().BeEquivalentTo([1, 4]);
    }

    [Fact]
    public async Task QueryWithBeforeTimestamp_ReturnsOnlyMessagesBeforeTimestamp()
    {
        // Arrange
        using var scope = CreateScope();
        var handler = scope.Get<IndexAction.Handler>();
        var ctx = scope.Get<AppDbContext>();
        SetupUser();

        var timestamp = DateTimeOffset.MinValue;
        var second = TimeSpan.FromSeconds(1);

        var messages = new List<Message>
        {
            MessageTemplate with {Id = 1, Timestamp = timestamp + second * 0},
            MessageTemplate with {Id = 2, Timestamp = timestamp + second * 1},
            MessageTemplate with {Id = 3, Timestamp = timestamp + second * 2},
        };

        ctx.TmiMessages.AddRange(messages);
        await ctx.SaveChangesAsync();

        var query = QueryTemplate with { BeforeTimestamp = messages[1].Timestamp };


        // Act
        var response = await handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResult().Subject.Value!
            .Select(x => x.Id)
            .Should().BeEquivalentTo([1, 2]);
    }
}

file static class Extensions
{
    public static AndWhichConstraint<ObjectAssertions, Ok<List<IndexAction.Result>>> BeOkResult(
        this ObjectAssertions assertions)
    {
        return assertions.BeOfType<Ok<List<IndexAction.Result>>>();
    }
}
