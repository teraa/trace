using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Trace.Api.Auth;
using Trace.Api.Twitch.Channels;
using Trace.Data.Models.Twitch;

namespace Trace.Tests.Twitch.Channels;

public class IndexActionTests(AppFactory appFactory) : AppTests(appFactory)
{

    [Fact]
    public async Task Index_Returns_AllowedChannels()
    {
        // Arrange
        AppFactory.UserAccessorMock.Setup(x => x.User)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim(AppClaimTypes.ChannelRead, "foo_id")])));

        using var scope = CreateScope();
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
            .Subject.Value.Should().BeEquivalentTo(new List<IndexAction.Result> { new("foo_id", "foo_login") });
    }
}
