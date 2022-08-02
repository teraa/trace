using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Trace.Api;
using Trace.Api.Extensions;
using Trace.Api.Options;
using Trace.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .UseSystemd();

builder.Services
    .AddAsyncInitialization()
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
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>))
    .AddValidatorsFromAssemblyContaining<Program>()
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
