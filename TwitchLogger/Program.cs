using MediatR;
using Microsoft.EntityFrameworkCore;
using Teraa.Twitch.PubSub;
using Teraa.Twitch.Tmi;
using Trace;
using Trace.Data;
using Trace.Initializers;
using Trace.Tmi;

var host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    })
    .ConfigureLogging((hostContext, builder) =>
    {
        builder.AddSeq(hostContext.Configuration.GetSection("Seq"));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddAsyncInitializer<MigrationInitializer>()
            .AddDbContext<TraceDbContext>((sp, options) =>
            {
                var dbOptions = sp
                    .GetRequiredService<IConfiguration>()
                    .GetOptions<DbOptions>();

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
            .AddOptionsWithSection<TmiOptions>(hostContext.Configuration)
            .AddTmiService()
            .AddOptionsWithSection<PubSubOptions>(hostContext.Configuration)
            .AddPubSubService()
            ;
    })
    .Build();

await host.InitAsync();
await host.RunAsync();
