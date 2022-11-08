using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Teraa.Extensions.AspNetCore;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Serilog.Seq;
using Teraa.Extensions.Serilog.Systemd;
using Trace.Api;
using Trace.Api.Features.Auth;
using Trace.Api.Options;
using Trace.Data;

var builder = WebApplication.CreateBuilder(args);

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
    .AddMediatR(typeof(Program))
    .AddRequestValidationBehaviour()
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddMemoryCache()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddOptionsWithValidation<DbOptions>()
    .AddOptionsWithValidation<JwtOptions>()
    .AddOptionsWithValidation<TwitchOptions>()
    .AddSingleton<TokenService>()
    .AddSingleton<JwtSigningKeyProvider>()
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
