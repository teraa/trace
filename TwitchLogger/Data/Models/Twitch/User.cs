using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch
{
    [PublicAPI]
    public class User
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public DateTimeOffset FirstSeenAt { get; set; }

        public ICollection<Message> AuthorMessages { get; set; }
        public ICollection<Message> ChannelMessages { get; set; }
        public ICollection<ModeratorAction> ChannelModeratorActions { get; set; }
        public ICollection<ModeratorAction> ModeratorModeratorActions { get; set; }
        public ICollection<ModeratorAction> TargetModeratorActions { get; set; }
        public ICollection<ChatLog> ChatLogs { get; set; }
        public ICollection<PubSubLog> PubSubLogs { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user", schema: "twitch");

            builder.HasIndex(x => x.Login);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<User> Users { get; init; }
    }
}
