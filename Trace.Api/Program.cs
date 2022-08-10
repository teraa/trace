using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Teraa.Extensions.AspNetCore;
using Teraa.Extensions.Configuration;
using Trace.Api;
using Trace.Api.Features.Auth;
using Trace.Api.Options;
using Trace.Data;

Serilog.Debugging.SelfLog
    .Enable(x => Console.WriteLine($"<4>SERILOG: {x}"));

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
        options.ReadFrom.Configuration(hostContext.Configuration);
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
        var dbOptions = services
            .GetRequiredService<IConfiguration>()
            .GetOptions<DbOptions>();

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
    .AddOptionsWithSection<TwitchOptions>(builder.Configuration)
    .AddOptionsWithSection<JwtOptions>(builder.Configuration)
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
