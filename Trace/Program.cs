using FluentValidation;
using Immediate.Handlers.Shared;
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
    .AddIdentity()
    .AddAuth(builder.Configuration)
    .AddSingleton<IUserAccessor, UserAccessor>()
    .AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>())
    .AddHttpContextAccessor()
    .AddTmi()
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
