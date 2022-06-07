using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Twitch
{
    [PublicAPI]
    public class MessageSource
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Message> Messages { get; set; }
    }

    public class MessageSourceConfiguration : IEntityTypeConfiguration<MessageSource>
    {
        public void Configure(EntityTypeBuilder<MessageSource> builder)
        {
            builder.Metadata.SetSchema("twitch");

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<MessageSource> MessageSources { get; init; }
    }
}
