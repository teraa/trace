using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data
{
    public class TwitchLoggerDbContext : DbContext
    {
        public TwitchLoggerDbContext(DbContextOptions<TwitchLoggerDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageSource> MessageSources { get; set; }
        public DbSet<ModeratorAction> ModeratorActions { get; set; }

        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<PubSubLog> PubSubLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", schema: "twitch");

                entity.HasIndex(x => x.Id);

                entity.HasIndex(x => x.Login);

                entity.Property(x => x.Id)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message", schema: "twitch");

                entity.HasIndex(x => x.ReceivedAt);

                entity.Property(x => x.AuthorId)
                    .IsRequired(false);

                entity.HasOne(x => x.Author)
                    .WithMany(x => x.Messages)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.ChannelMessages)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Source)
                    .WithMany(x => x.Messages)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessageSource>(entity =>
            {
                entity.ToTable("message_source", schema: "twitch");

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<ModeratorAction>(entity =>
            {
                entity.ToTable("moderator_action", schema: "twitch");

                entity.HasIndex(x => x.CreatedAt);

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.ChannelModeratorActions)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Moderator)
                    .WithMany(x => x.ModeratorActionsIssued)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Target)
                    .WithMany(x => x.ModeratorActionsReceived)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatLog>(entity =>
            {
                entity.ToTable("chat_log");

                entity.HasIndex(x => x.ChannelId)
                    .IsUnique();
            });

            modelBuilder.Entity<PubSubLog>(entity =>
            {
                entity.ToTable("pubsub_log");

                entity.HasIndex(x => x.Topic)
                    .IsUnique();
            });
        }
    }
}
