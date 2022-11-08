using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Data.Models.Twitch;

namespace Trace.Initializers;

[UsedImplicitly]
public class MigrationInitializer : IAsyncInitializer
{
    private readonly TraceDbContext _ctx;

    public MigrationInitializer(TraceDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task InitializeAsync()
    {
        await _ctx.Database.MigrateAsync();

        var since = await _ctx.TwitchUsers.MaxAsync(x => x.LastSeen);

        var users = await _ctx.TmiMessages
            .Where(x => x.Timestamp > since)
            .GroupBy(x => x.AuthorId)
            .Select(x => new
            {
                Id = x.Key,
                Entries = x.Select(static x => new
                    {
                        x.AuthorLogin,
                        x.Timestamp,
                    })
                    .OrderBy(static x => x.Timestamp)
                    .ToList()
            })
            .ToListAsync();

        var entries = new List<User>();

        foreach (var user in users)
        {
            User? entry = await _ctx.TwitchUsers
                .Where(x => x.Id == user.Id)
                .OrderByDescending(x => x.LastSeen)
                .FirstOrDefaultAsync();

            foreach (var userEntry in user.Entries)
            {
                if (entry?.Login == userEntry.AuthorLogin)
                {
                    entry.LastSeen = userEntry.Timestamp;
                }
                else
                {
                    entry = new User
                    {
                        Id = user.Id,
                        Login = userEntry.AuthorLogin,
                        FirstSeen = userEntry.Timestamp,
                        LastSeen = userEntry.Timestamp,
                    };
                    entries.Add(entry);
                }
            }
        }

        _ctx.TwitchUsers.AddRange(entries);
        await _ctx.SaveChangesAsync();
    }
}
