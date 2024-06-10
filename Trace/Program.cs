using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Settings.Configuration;
using Teraa.Extensions.AspNetCore;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Configuration.Vault.Options;
using Teraa.Extensions.Serilog.Seq;
using Teraa.Extensions.Serilog.Systemd;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace.Api;
using Trace.Api.Auth;
using Trace.Data;
using Trace.Options;
using Trace.PubSub;
using Trace.Tmi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddVault();

builder.Host
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .UseSystemd()
    .UseSerilog((hostContext, options) =>
    {
        options
            .ReadFrom.Configuration(hostContext.Configuration,
                new ConfigurationReaderOptions(typeof(ConsoleLoggerExtensions).Assembly))
            .ConfigureSystemdConsole()
            .ConfigureSeq(hostContext);
    });

builder.Services
    .AddAsyncInitializer<SourceInitializer>()
    .AddControllers(options =>
    {
        options.ModelValidatorProviders.Clear();
        options.ModelMetadataDetailsProviders.Add(new EmptyStringMetadataProvider());
    })
    .Services
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
    .AddTwitch(authOptions =>
    {
        var twitchOptions = builder.Configuration.GetValidatedRequiredOptions([new TwitchOptions.Validator()]);
        authOptions.ClientId = twitchOptions.ClientId;
        authOptions.ClientSecret = twitchOptions.ClientSecret;
        authOptions.CallbackPath = twitchOptions.CallbackPath;

        authOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
        authOptions.CorrelationCookie.Name = "Correlation.";

        // options.SaveTokens = true;
    })
    .Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(AppAuthzPolicies.Channel,
            policy => { policy.Requirements.Add(new ChannelAuthorizationHandler.Requirement()); });
    })
    .AddSingleton<IAuthorizationHandler, ChannelAuthorizationHandler>()
    .AddSingleton<IUserAccessor, UserAccessor>()
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<Program>();
    })
    .AddRequestValidationBehaviour()
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddMemoryCache()
    .AddSingleton<SourceProvider>()
    .AddSingleton<ISourceProvider>(services => services.GetRequiredService<SourceProvider>())
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddValidatedOptions<TwitchOptions>()
    .AddValidatedOptions<TmiOptions>()
    .AddValidatedOptions<PubSubOptions>()
    .AddTmiService()
    .AddPubSubService()
    .AddDb()
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

app.MapControllers();

await app.InitAsync();
await app.RunAsync();
