using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Pubsub;
using TwitchLogger.Data.Models.Tmi;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models
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
        public ICollection<Tmi.Config> TmiConfigs { get; set; }
        public ICollection<Pubsub.Config> PubsubConfigs { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
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
