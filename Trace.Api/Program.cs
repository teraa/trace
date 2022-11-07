using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Options;
using Serilog;
using Teraa.Extensions.AspNetCore;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Serilog;
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
            .Enrich.FromLogContext();

        var seqOptions = hostContext.Configuration.GetOptions(new[] {new SeqOptions.Validator()});
        if (seqOptions is { })
        {
            options.WriteTo.Seq(seqOptions.ServerUrl.ToString(), apiKey: seqOptions.ApiKey);
        }

        if (SystemdHelpers.IsSystemdService())
        {
            Serilog.Debugging.SelfLog
                .Enable(x => Console.WriteLine($"<4>SERILOG: {x}"));

            options
                .Enrich.With(new SyslogSeverityEnricher())
                .WriteTo.Console(outputTemplate: "<{SyslogSeverity}>{SourceContext}: {Message:j}{NewLine}");
        }
        else
        {
            Serilog.Debugging.SelfLog
                .Enable(x => Console.WriteLine($"SERILOG: {x}"));

            options.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
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
