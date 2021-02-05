using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data.Models;

namespace TwitchLogger.Data
{
    public class TwitchLoggerDbContext : DbContext
    {
        public TwitchLoggerDbContext(DbContextOptions<TwitchLoggerDbContext> options)
            : base(options) { }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<MessageSource> MessageSources { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user")
                    .HasKey(x => x.Id);

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
                entity.ToTable("message")
                    .HasKey(x => x.Id);

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
                    .HasColumnName("user_id")
                    .IsRequired(false);

                entity.Property(x => x.AuthorLogin)
                    .HasColumnName("user_login")
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
                entity.ToTable("message_source")
                    .HasKey(x => x.Id);

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
        }
    }
}
