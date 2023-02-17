using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Teraa.Extensions.AspNetCore;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Serilog.Seq;
using Teraa.Extensions.Serilog.Systemd;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace.Api;
using Trace.Api.Features.Auth;
using Trace.Data;
using Trace.Initializers;
using Trace.Options;
using Trace.Tmi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true)
    .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yml", optional: true, reloadOnChange: true);

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
            .ReadFrom.Configuration(hostContext.Configuration)
            .ConfigureSystemdConsole()
            .ConfigureSeq(hostContext);
    });

builder.Services
    .AddAsyncInitialization()
    .AddAsyncInitializer<MigrationInitializer>()
    .ConfigureOptions<ConfigureJwtBearerOptions>()
    .AddAuthentication(options =>
    {
        // https://stackoverflow.com/a/46224126
        options.DefaultAuthenticateScheme = AppAuthScheme.Bearer;
        options.DefaultChallengeScheme = AppAuthScheme.Bearer;
    })
    .AddJwtBearer(AppAuthScheme.Bearer, _ => { })
    .AddJwtBearer(AppAuthScheme.ExpiredBearer, _ => { })
    .Services
    .AddControllers(options =>
    {
        options.ModelValidatorProviders.Clear();
        options.ModelMetadataDetailsProviders.Add(new EmptyStringMetadataProvider());

    })
    .Services
    .AddDbContext<TraceDbContext>((services, options) =>
    {
        using var scope = services.CreateScope();
        var dbOptions = scope.ServiceProvider
            .GetRequiredService<IOptionsMonitor<DbOptions>>()
            .CurrentValue;

        options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
        {
            contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

#if DEBUG
        options.EnableSensitiveDataLogging();
#endif
    })
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<Program>();
    })
    .AddRequestValidationBehaviour()
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddMemoryCache()
    .AddSingleton<SourceCache>()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddOptionsWithValidation<DbOptions>()
    .AddOptionsWithValidation<JwtOptions>()
    .AddOptionsWithValidation<TwitchOptions>()
    .AddOptionsWithValidation<TmiOptions>()
    .AddOptionsWithValidation<PubSubOptions>()
    .AddSingleton<TokenService>()
    .AddSingleton<JwtSigningKeyProvider>()
    .AddTmiService()
    .AddPubSubService()
    ;

var app = builder.Build();

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
