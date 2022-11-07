using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Options;
using Serilog;
using Teraa.Extensions.Configuration;
using Teraa.Extensions.Serilog;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace.Data;
using Trace.Initializers;
using Trace.Options;
using Trace.Tmi;

var host = Host.CreateDefaultBuilder(args)
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
    })
    // ReSharper disable once UnusedParameter.Local
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddValidatorsFromAssemblyContaining<Program>()
            .AddAsyncInitializer<MigrationInitializer>()
            .AddOptionsWithValidation<DbOptions>()
            .AddDbContext<TraceDbContext>((sp, options) =>
            {
                using var scope = sp.CreateScope();
                var dbOptions = scope.ServiceProvider
                    .GetRequiredService<IOptionsMonitor<DbOptions>>()
                    .CurrentValue;

                options.UseNpgsql(dbOptions.ConnectionString, contextOptions =>
                {
                    contextOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    contextOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            })
            .AddMemoryCache()
            .AddMediatR(typeof(Program))
            .AddSingleton<SourceCache>()
            .AddOptionsWithValidation<TmiOptions>()
            .AddTmiService()
            .AddOptionsWithValidation<PubSubOptions>()
            .AddPubSubService()
            ;
    })
    .Build();

await host.InitAsync();
await host.RunAsync();
