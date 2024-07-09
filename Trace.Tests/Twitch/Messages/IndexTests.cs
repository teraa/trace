using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api;
using Trace.Data;
using Trace.Data.Models.Tmi;
using Index = Trace.Api.Twitch.Messages.Actions.Index;

namespace Trace.Tests.Twitch.Messages;

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

    private void SetChannelRead(string channelId)
    {
        _appFactory.SetUser([new Claim(AppClaimTypes.ChannelRead, channelId)]);
    }


    [Fact]
    public async Task Forbidden_WhenUnauthorized()
    {
        SetChannelRead("foo");
        var request = new Index.Query(
            ChannelId: "bar",
            Limit: 1
        );

        var actionResult = await _handler.HandleAsync(request);

        actionResult.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Returns_CorrectChannelMessages()
    {
        const string channelId = "channel.id";
        SetChannelRead(channelId);
        var template = new Message
        {
            AuthorId = "author.id",
            AuthorLogin = "author.login",
            ChannelId = channelId,
            Content = "Lorem ipsum",
            Source = new Source {Name = "source"},
            Timestamp = DateTimeOffset.MinValue,
        };

        var messages = new List<Message>
        {
            template with {Id = 1},
            template with {Id = 2, ChannelId = "foo"},
            template with {Id = 3, ChannelId = "bar"},
            template with {Id = 4},
            template with {Id = 5, ChannelId = "foo"},
            template with {Id = 6, ChannelId = "bar"},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(channelId);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Select(x => x.Id)
            .Should().BeEquivalentTo([1, 4]);
    }

    [Fact]
    public async Task QueryWithBefore_RespectsTimestampNotId()
    {
        const string channelId = "channel.id";
        SetChannelRead(channelId);
        var start = DateTimeOffset.MinValue;
        var second = new TimeSpan(0, 0, 1);
        var template = new Message
        {
            AuthorId = "author.id",
            AuthorLogin = "author.login",
            ChannelId = channelId,
            Content = "Lorem ipsum",
            Source = new Source {Name = "source"},
            Timestamp = start,
        };

        var messages = new List<Message>
        {
            template with {Id = 1, Timestamp = start + second * 1}, // before
            template with {Id = 5, Timestamp = start + second * 2}, // before
            template with {Id = 3, Timestamp = start + second * 3}, // before
            template with {Id = 2, Timestamp = start + second * 4}, // pivot
            template with {Id = 4, Timestamp = start + second * 5}, // after
        };

        _ctx.TmiMessages.AddRange(messages);

        await _ctx.SaveChangesAsync();

        var query = new Index.Query(template.ChannelId, Before: 2);


        // Act
        var response = await _handler.HandleAsync(query);


        var results = response.Should().BeResults().Subject;

        results.Select(x => x.Id)
            .Should().Equal([3, 5, 1]);
    }

    [Fact]
    public async Task QueryWithInvalidBefore_ReturnsBadRequest()
    {
        const string channelId = "channel.id";
        SetChannelRead(channelId);
        var template = new Message
        {
            AuthorId = "author.id",
            AuthorLogin = "author.login",
            ChannelId = channelId,
            Content = "Lorem ipsum",
            Source = new Source {Name = "source"},
            Timestamp = DateTimeOffset.MinValue,
        };

        var messages = new List<Message>
        {
            template with {Id = 1},
            template with {Id = 2, ChannelId = "other"},
        };

        _ctx.TmiMessages.AddRange(messages);

        await _ctx.SaveChangesAsync();

        var query = new Index.Query(template.ChannelId, Before: 2);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task QueryWithAuthorId_ReturnsOnlyMessagesByAuthorId()
    {
        const string channelId = "channel.id";
        SetChannelRead(channelId);
        var template = new Message
        {
            AuthorId = "author.id",
            AuthorLogin = "author.login",
            ChannelId = channelId,
            Content = "Lorem ipsum",
            Source = new Source {Name = "source"},
            Timestamp = DateTimeOffset.MinValue,
        };

        var messages = new List<Message>
        {
            template with {Id = 1, AuthorId = "1"}, // target
            template with {Id = 2, AuthorId = "2"},
            template with {Id = 3, AuthorId = "3"},
            template with {Id = 4, AuthorId = "1"}, // target
            template with {Id = 5, AuthorId = "5"},
        };

        _ctx.TmiMessages.AddRange(messages);

        await _ctx.SaveChangesAsync();

        const string authorId = "1";
        var query = new Index.Query(template.ChannelId, AuthorId: authorId);


        // Act
        var response = await _handler.HandleAsync(query);


        var results = response.Should().BeResults().Subject;

        results.Select(x => x.AuthorId)
            .Should().AllBe(authorId);

        results.Select(x => x.Id)
            .Should().BeEquivalentTo([1, 4]);
    }

    [Fact]
    public async Task QueryWithBeforeTimestamp_ReturnsOnlyMessagesBeforeTimestamp()
    {
        const string channelId = "channel.id";
        SetChannelRead(channelId);

        var timestamp = DateTimeOffset.MinValue;
        var second = TimeSpan.FromSeconds(1);

        var template = new Message
        {
            AuthorId = "author.id",
            AuthorLogin = "author.login",
            ChannelId = channelId,
            Content = "Lorem ipsum",
            Source = new Source {Name = "source"},
            Timestamp = timestamp,
        };

        var messages = new List<Message>
        {
            template with {Id = 1, Timestamp = timestamp + second * 0},
            template with {Id = 2, Timestamp = timestamp + second * 1},
            template with {Id = 3, Timestamp = timestamp + second * 2},
        };

        _ctx.TmiMessages.AddRange(messages);
        await _ctx.SaveChangesAsync();

        var query = new Index.Query(channelId, BeforeTimestamp: messages[1].Timestamp);


        // Act
        var response = await _handler.HandleAsync(query);


        response.Should().BeResults()
            .Subject.Select(x => x.Id)
            .Should().BeEquivalentTo([1, 2]);
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
