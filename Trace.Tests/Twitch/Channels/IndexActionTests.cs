using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Trace.Api.Auth;
using Trace.Api.Twitch.Channels;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Tests.Twitch.Channels;

[Collection("1")]
public class IndexActionTests
{
    private readonly AppFactory _appFactory;

    public IndexActionTests(AppFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task Index_Returns_AllowedChannels()
    {
        // Arrange
        _appFactory.UserAccessorMock.Setup(x => x.User)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim(AppClaimTypes.ChannelRead, "foo_id")])));

        using var scope = _appFactory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IndexAction.Handler>();
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

        var request = new IndexAction.Query();


        // Act
        var actionResult = await handler.HandleAsync(request);


        // Assert
        actionResult.Should().BeOfType<Ok<List<IndexAction.Result>>>()
            .Subject.Value.Should().BeEquivalentTo(new List<IndexAction.Result> {new("foo_id", "foo_login")});
    }
}
