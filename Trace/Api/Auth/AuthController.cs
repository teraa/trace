using System.Security.Claims;
using AspNet.Security.OAuth.Twitch;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Trace.Data;
using Trace.Options;

namespace Trace.Api.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IOptionsMonitor<TwitchOptions> _options;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AuthController> logger, IOptionsMonitor<TwitchOptions> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _options = options;
    }

    [HttpGet("[action]")]
    public async Task Login()
    {
        // Clear the existing external cookie to ensure a clean login process
        // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            "Twitch",
            "Auth/Continue");

        await HttpContext.ChallengeAsync(TwitchAuthenticationDefaults.AuthenticationScheme, properties);
    }

    // Both Identity.External and Twitch schemes will work here,
    // but Twitch will redirect to twitch.tv and External will redirect to /Account/Login...
    // when request is unauthenticated
    [Authorize(AuthenticationSchemes = TwitchAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet("[action]")]
    public async Task<IActionResult> Continue()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            _logger.LogWarning("Couldn't load external login information");
            return BadRequest();
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true);

        // Existing user
        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User {ProviderKey} logged in with {LoginProvider}", info.ProviderKey, info.LoginProvider);
            return Redirect(_options.CurrentValue.RedirectUri.ToString());
        }

        if (signInResult.IsLockedOut)
        {
            return Forbid();
        }

        // New user
        var user = new AppUser();
        var twitchLogin = User.Identity!.Name!;
        var twitchId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
        await _userManager.SetUserNameAsync(user, $"{twitchLogin}:{twitchId.Value}"); // Whatever, not important

        var identityResult = await _userManager.CreateAsync(user);
        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Failed to create user: {Errors}", identityResult.Errors.Select(x => x.Description));
            return BadRequest();
        }

        identityResult = await _userManager.AddClaimsAsync(user,
        [
            new Claim(AppClaimTypes.TwitchId, twitchId.Value),
            new Claim(AppClaimTypes.TwitchLogin, twitchLogin)
        ]);

        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Failed to add user claims: {Errors}", identityResult.Errors.Select(x => x.Description));
            return BadRequest();
        }

        identityResult = await _userManager.AddLoginAsync(user, info);
        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Failed to add login for user: {Errors}", identityResult.Errors.Select(x => x.Description));
            return BadRequest();
        }

        signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User {ProviderKey} created an account with {LoginProvider}", info.ProviderKey, info.LoginProvider);
            return Redirect(_options.CurrentValue.RedirectUri.ToString());
        }

        return BadRequest(signInResult);
    }

    [HttpPost("[action]")]
    public async Task Logout()
    {
        // This just deletes the cookie client side, but it's still valid
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

    [Authorize]
    [HttpGet("[action]")]
    public IActionResult Info()
    {
        return Ok(User.Identities.Select(i => new
        {
            i.AuthenticationType,
            Claims = i.Claims.Select(c => new
            {
                c.Type,
                c.Value
            })
        }));
    }

    [HttpGet("[action]")]
    public IActionResult Test()
    {
        return Ok(new
        {
            Scheme = Request.Scheme.ToString(),
            Host = Request.Host.ToString(),
            Path = Request.Path.ToString(),
            PathBase = Request.PathBase.ToString(),
            Request.Headers,
            Connection = new
            {
                RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Connection.RemotePort,
            },
        });
    }
}
