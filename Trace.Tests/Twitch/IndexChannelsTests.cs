using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api;
using Trace.Data;
using Trace.Data.Models.Twitch;
using Index = Trace.Api.Twitch.Channels.Actions.Index;

namespace Trace.Tests.Twitch;

public class IndexChannelsTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _appFactory;

    public IndexChannelsTests(AppFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task Index_Returns_AllowedChannels()
    {
        _appFactory.SetUser([new Claim(AppClaimTypes.ChannelRead, "foo_id")]);
        using var scope = _appFactory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Index.Handler>();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ctx.TwitchUsers.AddRange([
            new User
            {
                Id = "foo_id",
                Login = "foo_login",
            },
            new User
            {
                Id = "bar_id",
                Login = "bar_login",
            },
        ]);
        await ctx.SaveChangesAsync();

        var request = new Index.Query();


        var actionResult = await handler.HandleAsync(request);


        var results = actionResult.Should().BeOkObjectResult<List<Index.Result>>().Subject;
        results.Should().HaveCount(1);
        results[0].Should().BeEquivalentTo(new Index.Result("foo_id", "foo_login"));
    }
}

file static class Extensions
{
    public static AndWhichConstraint<ObjectAssertions, TResult> BeOkObjectResult<TResult>(
        this ObjectAssertions assertions)
    {
        return assertions.BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<TResult>();
    }
}
