using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Tmi;

#pragma warning disable CS8618
namespace TwitchLogger.Data.Models.Tmi
{
    [PublicAPI]
    public class Message
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public short SourceId { get; set; }
        public string ChannelId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorLogin { get; set; }
        public string Content { get; set; }

        public User Author { get; set; }
        public User Channel { get; set; }
        public Source Source { get; set; }
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Metadata.SetSchema("tmi");
            builder.Metadata.SetTableName("messages");

            builder.HasIndex(x => x.Timestamp);

            builder.HasOne(x => x.Author)
                .WithMany(x => x.AuthorMessages)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Channel)
                .WithMany(x => x.ChannelMessages)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Source)
                .WithMany(x => x.Messages)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

namespace TwitchLogger.Data
{
    public partial class TwitchLoggerDbContext
    {
        public DbSet<Message> TmiMessages { get; init; }
    }
}
