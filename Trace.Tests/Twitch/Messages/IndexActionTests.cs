using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api;
using Trace.Api.Twitch.Messages;
using Trace.Data;
using Trace.Data.Models.Tmi;

namespace Trace.Tests.Twitch.Messages;

[Collection("1")]
public sealed class IndexActionTests : IAsyncLifetime, IDisposable
{
    private const string s_channelId = "channel.id";

    private readonly AppFactory _appFactory;
    private readonly IServiceScope _scope;
    private readonly IndexAction.Handler _handler;
#pragma warning disable CA2213
    private readonly AppDbContext _ctx;
#pragma warning restore CA2213

    public IndexActionTests(AppFactory appFactory)
    {
        _appFactory = appFactory;
        _scope = _appFactory.Services.CreateScope();
        _handler = _scope.ServiceProvider.GetRequiredService<IndexAction.Handler>();
        _ctx = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _appFactory.SetUser([new Claim(AppClaimTypes.ChannelRead, ChannelId)]);
    }

    private static string ChannelId => s_channelId;

    private Message Template { get; } = new()
    {
        AuthorId = "author.id",
        AuthorLogin = "author.login",
        ChannelId = "channel.id",
        Content = "Lorem ipsum",
        Source = new Source {Name = "source"},
        Timestamp = DateTimeOffset.MinValue,
    };

    private IndexAction.Query Query { get; } = new(s_channelId);

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
    public async Task Forbidden_WhenUnauthorized()
    {
        var request = new IndexAction.Query("foo");

        var actionResult = await _handler.HandleAsync(request);

        actionResult.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public async Task Returns_CorrectChannelMessages()
    {
        // Arrange
        var messages = new List<Message>
        {
            Template with {Id = 1},
            Template with {Id = 2, ChannelId = "foo"},
            Template with {Id = 3, ChannelId = "bar"},
            Template with {Id = 4},
            Template with {Id = 5, ChannelId = "foo"},
            Template with {Id = 6, ChannelId = "bar"},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();


        // Act
        var response = await _handler.HandleAsync(Query);


        // Assert
        response.Should().BeOkResult().Subject.Value!
            .Select(x => x.Id)
            .Should().BeEquivalentTo([1, 4]);
    }

    [Fact]
    public async Task QueryWithBefore_RespectsTimestampNotId()
    {
        // Arrange
        var start = DateTimeOffset.MinValue;
        var second = new TimeSpan(0, 0, 1);

        var messages = new List<Message>
        {
            Template with {Id = 1, Timestamp = start + second * 1}, // before
            Template with {Id = 5, Timestamp = start + second * 2}, // before
            Template with {Id = 3, Timestamp = start + second * 3}, // before
            Template with {Id = 2, Timestamp = start + second * 4}, // pivot
            Template with {Id = 4, Timestamp = start + second * 5}, // after
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        var query = Query with {Before = 2};


        // Act
        var response = await _handler.HandleAsync(query);


        // Assert
        response.Should().BeOkResult().Subject.Value!
            .Select(x => x.Id)
            .Should().Equal([3, 5, 1]);
    }

    [Fact]
    public async Task QueryWithInvalidBefore_ReturnsBadRequest()
    {
        // Arrange
        var messages = new List<Message>
        {
            Template with {Id = 1},
            Template with {Id = 2, ChannelId = "foo"},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        var query = Query with {Before = 2};


        // Act
        var response = await _handler.HandleAsync(query);


        // Assert
        response.Should().BeOfType<ProblemHttpResult>().Subject
            .StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task QueryWithAuthorId_ReturnsOnlyMessagesByAuthorId()
    {
        // Arrange
        var messages = new List<Message>
        {
            Template with {Id = 1, AuthorId = "1"}, // target
            Template with {Id = 2, AuthorId = "2"},
            Template with {Id = 3, AuthorId = "3"},
            Template with {Id = 4, AuthorId = "1"}, // target
            Template with {Id = 5, AuthorId = "5"},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        const string authorId = "1";
        var query = Query with {AuthorId = authorId};


        // Act
        var response = await _handler.HandleAsync(query);


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
        var timestamp = DateTimeOffset.MinValue;
        var second = TimeSpan.FromSeconds(1);

        var messages = new List<Message>
        {
            Template with {Id = 1, Timestamp = timestamp + second * 0},
            Template with {Id = 2, Timestamp = timestamp + second * 1},
            Template with {Id = 3, Timestamp = timestamp + second * 2},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        var query = Query with {BeforeTimestamp = messages[1].Timestamp};


        // Act
        var response = await _handler.HandleAsync(query);


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
