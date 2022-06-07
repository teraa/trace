using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618

namespace TwitchLogger.Data.Models.Twitch
{
    [PublicAPI]
    public class Message
    {
        public long Id { get; set; }
        public DateTimeOffset ReceivedAt { get; set; }
        public short SourceId { get; set; }
        public string ChannelId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorLogin { get; set; }
        public string Content { get; set; }

        public User Author { get; set; }
        public User Channel { get; set; }
        public MessageSource Source { get; set; }
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("message", schema: "twitch");

            builder.HasIndex(x => x.ReceivedAt);

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
        public DbSet<Message> Messages { get; init; }
    }
}
