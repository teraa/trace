using System.Globalization;
using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api;
using Trace.Data;
using Trace.Data.Models.Tmi;
using Index = Trace.Api.Twitch.Messages.Actions.Index;

namespace Trace.Tests.Twitch;

[Collection("1")]
public sealed class IndexMessagesTests : IAsyncLifetime, IDisposable
{
    private readonly AppFactory _appFactory;
    private readonly IServiceScope _scope;
    private readonly Index.Handler _handler;
#pragma warning disable CA2213
    private readonly AppDbContext _ctx;
#pragma warning restore CA2213

    public IndexMessagesTests(AppFactory appFactory)
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
    public async Task Index_ReturnsForbidden_WhenUnauthorized()
    {
        var channelIds = new[] {"10", "20"};
        SetChannelRead(channelIds[0]);
        var request = new Index.Query(
            ChannelId: channelIds[1],
            Limit: 1
        );

        var actionResult = await _handler.HandleAsync(request);

        actionResult.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Index_ReturnsCorrectChannelMessages()
    {
        var channelIds = new[] {"10", "20", "30"};
        _appFactory.SetUser([new Claim(AppClaimTypes.ChannelRead, channelIds[0])]);

        var source = new Source
        {
            Name = "Trace",
        };

        _ctx.TmiSources.Add(source);

        Randomizer.Seed = new Random(1337);
        var messages = new Faker<Message>()
            .RuleFor(x => x.Timestamp, x => x.Date.RecentOffset().ToUniversalTime())
            .RuleFor(x => x.ChannelId, x => x.PickRandom(channelIds))
            .RuleFor(x => x.AuthorId, x => x.Random.Number(1_000, 10_000).ToString(CultureInfo.InvariantCulture))
            .RuleFor(x => x.AuthorLogin, x => x.Internet.UserName().ToLowerInvariant())
            .RuleFor(x => x.Content, x => x.Lorem.Sentence())
            .RuleFor(x => x.Source, x => source)
            .Generate(100);

        messages
            .Select(x => x.ChannelId)
            .Distinct()
            .Should().HaveCount(channelIds.Length);

        _ctx.TmiMessages.AddRange(messages);

        var request = new Index.Query(
            ChannelId: channelIds[0],
            Limit: 100
        );

        await _ctx.SaveChangesAsync();


        var actionResult = await _handler.HandleAsync(request);


        var results = actionResult
            .Should().BeOfType<OkObjectResult>().Subject.Value
            .Should().BeOfType<List<Index.Result>>().Subject;

        var validMessageIds = messages
            .OrderByDescending(x => x.Timestamp)
            .Where(x => x.ChannelId == channelIds[0])
            .Select(x => x.Id)
            .ToList();

        results.Select(x => x.Id)
            .Should().NotBeEmpty()
            .And.BeEquivalentTo(validMessageIds);
    }
}
