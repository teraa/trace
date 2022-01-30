using Microsoft.EntityFrameworkCore;
using TwitchLogger.Data.Models;
using Twitch = TwitchLogger.Data.Models.Twitch;

#pragma warning disable CS8618
namespace TwitchLogger.Data;

    public class TwitchLoggerDbContext : DbContext
    {
        public TwitchLoggerDbContext(DbContextOptions<TwitchLoggerDbContext> options)
            : base(options) { }

        public DbSet<Twitch.User> Users { get; set; }
        public DbSet<Twitch.Message> Messages { get; set; }
        public DbSet<Twitch.MessageSource> MessageSources { get; set; }
        public DbSet<Twitch.ModeratorAction> ModeratorActions { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<PubSubLog> PubSubLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Twitch.User>(x => x.ToTable("user", schema: "twitch"));
            modelBuilder.Entity<Twitch.Message>(x => x.ToTable("message", schema: "twitch"));
            modelBuilder.Entity<Twitch.MessageSource>(x => x.ToTable("message_source", schema: "twitch"));
            modelBuilder.Entity<Twitch.ModeratorAction>(x => x.ToTable("moderator_action", schema: "twitch"));
            modelBuilder.Entity<ChatLog>(x => x.ToTable("chat_log"));
            modelBuilder.Entity<PubSubLog>(x => x.ToTable("pubsub_log"));

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TwitchLoggerDbContext).Assembly);
        }
    }
