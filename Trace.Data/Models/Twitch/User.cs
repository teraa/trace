using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Data.Models.Twitch;

#pragma warning disable CS8618
namespace Trace.Data.Models.Twitch
{
    [PublicAPI]
    public class User
    {
        public long EntryId { get; set; }
        public string Id { get; set; }
        public string Login { get; set; }
        public DateTimeOffset FirstSeen { get; set; }
        public DateTimeOffset LastSeen { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Metadata.SetSchema("twitch");
            builder.Metadata.SetTableName("users");

            builder.HasKey(x => x.EntryId);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Login);
        }
    }
}

namespace Trace.Data
{
    public partial class TraceDbContext
    {
        public DbSet<User> TwitchUsers { get; init; }
    }
}
