using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data.Models;
using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data
{
    public class TwitchLoggerDbContext : DbContext
    {
        public TwitchLoggerDbContext(DbContextOptions<TwitchLoggerDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<MessageSource> MessageSources { get; set; } = null!;
        public DbSet<ModeratorAction> ModeratorActions { get; set; } = null!;

        public DbSet<ChatLog> ChatLogs { get; set; } = null!;
        public DbSet<PubSubLog> PubSubLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", schema: "twitch")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.Id);

                entity.HasIndex(x => x.Login);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedNever();

                entity.Property(x => x.Login)
                    .HasColumnName("login")
                    .IsRequired();

                entity.Property(x => x.FirstSeenAt)
                    .HasColumnName("first_seen_at")
                    .IsRequired();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message", schema: "twitch")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.ReceivedAt);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ReceivedAt)
                    .HasColumnName("received_at")
                    .IsRequired();

                entity.Property(x => x.SourceId)
                    .HasColumnName("source_id")
                    .IsRequired();

                entity.Property(x => x.ChannelId)
                    .HasColumnName("channel_id")
                    .IsRequired();

                entity.Property(x => x.AuthorId)
                    .HasColumnName("author_id")
                    .IsRequired(false);

                entity.Property(x => x.AuthorLogin)
                    .HasColumnName("author_login")
                    .IsRequired();

                entity.Property(x => x.Content)
                    .HasColumnName("content")
                    .IsRequired();

                entity.HasOne(x => x.Author)
                    .WithMany(x => x.Messages)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.ChannelMessages)
                    .HasForeignKey(x => x.ChannelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Source)
                    .WithMany(x => x.Messages)
                    .HasForeignKey(x => x.SourceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessageSource>(entity =>
            {
                entity.ToTable("message_source", schema: "twitch")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.Name)
                    .IsUnique();

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnName("description")
                    .IsRequired(false);
            });

            modelBuilder.Entity<ModeratorAction>(entity =>
            {
                entity.ToTable("moderator_action", schema: "twitch")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.CreatedAt);

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired();

                entity.Property(x => x.ChannelId)
                    .HasColumnName("channel_id")
                    .IsRequired();

                entity.Property(x => x.Action)
                    .HasColumnName("action")
                    .IsRequired();

                entity.Property(x => x.Args)
                    .HasColumnName("args")
                    .IsRequired(false);

                entity.Property(x => x.MessageId)
                    .HasColumnName("message_id")
                    .IsRequired(false);

                entity.Property(x => x.ModeratorId)
                    .HasColumnName("moderator_id")
                    .IsRequired(false);

                entity.Property(x => x.TargetId)
                    .HasColumnName("target_id")
                    .IsRequired(false);

                entity.Property(x => x.TargetLogin)
                    .HasColumnName("target_login")
                    .IsRequired(false);

                entity.Property(x => x.ModeratorMessage)
                    .HasColumnName("moderator_message")
                    .IsRequired(false);

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.ChannelModeratorActions)
                    .HasForeignKey(x => x.ChannelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Moderator)
                    .WithMany(x => x!.ModeratorActionsIssued)
                    .HasForeignKey(x => x.ModeratorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Target)
                    .WithMany(x => x!.ModeratorActionsReceived)
                    .HasForeignKey(x => x.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatLog>(entity =>
            {
                entity.ToTable("chat_log")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.ChannelId)
                    .IsUnique();

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ChannelId)
                    .HasColumnName("channel_id")
                    .IsRequired();

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.ChatLogs)
                    .HasForeignKey(x => x.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PubSubLog>(entity =>
            {
                entity.ToTable("pubsub_log")
                    .HasKey(x => x.Id);

                entity.HasIndex(x => x.Topic)
                    .IsUnique();

                entity.Property(x => x.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Topic)
                    .HasColumnName("topic")
                    .IsRequired();

                entity.Property(x => x.ChannelId)
                    .HasColumnName("channel_id")
                    .IsRequired();

                entity.HasOne(x => x.Channel)
                    .WithMany(x => x.PubSubLogs)
                    .HasForeignKey(x => x.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
