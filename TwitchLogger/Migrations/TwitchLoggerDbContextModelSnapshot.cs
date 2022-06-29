﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TwitchLogger.Data;

#nullable disable

namespace TwitchLogger.Migrations
{
    [DbContext(typeof(TwitchLoggerDbContext))]
    partial class TwitchLoggerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("topic");

                    b.HasKey("Id")
                        .HasName("pk_configs");

                    b.HasIndex("Topic")
                        .IsUnique()
                        .HasDatabaseName("ix_configs_topic");

                    b.ToTable("configs", "pubsub");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.ModeratorAction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("action");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("discriminator");

                    b.Property<string>("InitiatorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("initiator_id");

                    b.Property<string>("InitiatorName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("initiator_name");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("Id")
                        .HasName("pk_moderator_actions");

                    b.HasIndex("Action")
                        .HasDatabaseName("ix_moderator_actions_action");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_moderator_actions_channel_id");

                    b.HasIndex("InitiatorId")
                        .HasDatabaseName("ix_moderator_actions_initiator_id");

                    b.HasIndex("Timestamp")
                        .HasDatabaseName("ix_moderator_actions_timestamp");

                    b.ToTable("moderator_actions", "pubsub");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ModeratorAction");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Tmi.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("ChannelLogin")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_login");

                    b.HasKey("Id")
                        .HasName("pk_configs");

                    b.HasIndex("ChannelId")
                        .IsUnique()
                        .HasDatabaseName("ix_configs_channel_id");

                    b.ToTable("configs", "tmi");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Tmi.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("author_id");

                    b.Property<string>("AuthorLogin")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("author_login");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<short>("SourceId")
                        .HasColumnType("smallint")
                        .HasColumnName("source_id");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("Id")
                        .HasName("pk_messages");

                    b.HasIndex("AuthorId")
                        .HasDatabaseName("ix_messages_author_id");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_messages_channel_id");

                    b.HasIndex("SourceId")
                        .HasDatabaseName("ix_messages_source_id");

                    b.HasIndex("Timestamp")
                        .HasDatabaseName("ix_messages_timestamp");

                    b.ToTable("messages", "tmi");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Tmi.Source", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_sources");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_sources_name");

                    b.ToTable("sources", "tmi");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Followers", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.ModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration");

                    b.HasDiscriminator().HasValue("Followers");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Raid", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.ModeratorAction");

                    b.Property<string>("TargetName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("target_name");

                    b.HasDiscriminator().HasValue("Raid");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Slow", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.ModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("slow_duration");

                    b.HasDiscriminator().HasValue("Slow");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.TargetedModeratorAction", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.ModeratorAction");

                    b.Property<string>("TargetId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("target_id");

                    b.Property<string>("TargetName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("targeted_moderator_action_target_name");

                    b.HasIndex("TargetId")
                        .HasDatabaseName("ix_moderator_actions_target_id");

                    b.HasDiscriminator().HasValue("TargetedModeratorAction");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.TermAction", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.ModeratorAction");

                    b.Property<string>("TermId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("term_id");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasDiscriminator().HasValue("TermAction");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Ban", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.HasDiscriminator().HasValue("Ban");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Delete", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("MessageId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_id");

                    b.HasDiscriminator().HasValue("Delete");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.Timeout", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("timeout_duration");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("timeout_reason");

                    b.HasDiscriminator().HasValue("Timeout");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Pubsub.UnbanRequestAction", b =>
                {
                    b.HasBaseType("TwitchLogger.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<string>("ModeratorMessage")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("moderator_message");

                    b.HasDiscriminator().HasValue("UnbanRequestAction");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Tmi.Message", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.Tmi.Source", "Source")
                        .WithMany("Messages")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_messages_tmi_sources_source_id");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Tmi.Source", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
