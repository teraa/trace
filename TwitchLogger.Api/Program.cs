using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TwitchLogger.Api.Extensions;
using TwitchLogger.Api.Options;
using TwitchLogger.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .UseSystemd();

builder.Services
    .AddControllers()
    .Services
    .AddAsyncInitialization()
    .AddMediatR(typeof(Program))
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddDbContext<TwitchLoggerDbContext>((services, options) =>
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
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.InitAsync();
await app.RunAsync();
