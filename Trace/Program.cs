using FluentValidation;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Teraa.Shared.AspNetCore;
using Teraa.Shared.AspNetCore.MinimalApis;
using Teraa.Shared.Configuration.Vault;
using Teraa.Shared.Serilog.Seq;
using Teraa.Shared.Serilog.Systemd;
using Trace;
using Trace.Api;
using Trace.Api.Auth;
using Trace.Data;
using Trace.PubSub;
using Trace.Tmi;

[assembly: Behaviors(typeof(RequestValidationBehavior<,>))]

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddVault();

builder.Host
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .UseSystemd();

builder.Logging
    .ClearProviders()
    .AddSerilog(
        new LoggerConfiguration()
            .ConfigureDefaultLoggerConfiguration(builder.Configuration)
            .ConfigureSeq(builder.Configuration)
            .CreateLogger()
    );

builder.Services
    // .AddEndpointsApiExplorer()
    .AddCors()
    .AddRequestValidationBehaviour()
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddDb()
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters += ":";
        // options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    // .AddUserConfirmation<>()
    .Services
    .ConfigureApplicationCookie(options => { options.Cookie.Name = "Auth"; })
    .ConfigureExternalCookie(options => { options.Cookie.Name = "External"; })
    .AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = CustomAuthenticationHandler.SchemeName;
        options.DefaultForbidScheme = CustomAuthenticationHandler.SchemeName;
        options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationHandler.SchemeName, null);
    })
    .AddTwitchAuth(builder.Configuration)
    .Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(AppAuthzPolicies.Channel,
            policy => { policy.Requirements.Add(new ChannelAuthorizationHandler.Requirement()); });
    })
    .AddSingleton<IAuthorizationHandler, ChannelAuthorizationHandler>()
    .AddSingleton<IUserAccessor, UserAccessor>()
    .AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>())
    .AddHttpContextAccessor()
    .AddTmi()
    .AddPubSub()
    .AddTraceHandlers()
    .AddTraceBehaviors()
    ;

var app = builder.Build();

app.UseForwardedHeaders(app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapApi();

await app.InitAsync();
await app.RunAsync();

public partial class Program;
