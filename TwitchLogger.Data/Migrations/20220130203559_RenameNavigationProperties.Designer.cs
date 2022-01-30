﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TwitchLogger.Data;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    [DbContext(typeof(TwitchLoggerDbContext))]
    [Migration("20220130203559_RenameNavigationProperties")]
    partial class RenameNavigationProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TwitchLogger.Data.Models.ChatLog", b =>
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

                    b.HasKey("Id")
                        .HasName("pk_chat_log");

                    b.HasIndex("ChannelId")
                        .IsUnique()
                        .HasDatabaseName("ix_chat_log_channel_id");

                    b.ToTable("chat_log", (string)null);
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.PubSubLog", b =>
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
                        .HasName("pk_pubsub_log");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_pubsub_log_channel_id");

                    b.HasIndex("Topic")
                        .IsUnique()
                        .HasDatabaseName("ix_pubsub_log_topic");

                    b.ToTable("pubsub_log", (string)null);
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.Message", b =>
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

                    b.Property<DateTimeOffset>("ReceivedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("received_at");

                    b.Property<short>("SourceId")
                        .HasColumnType("smallint")
                        .HasColumnName("source_id");

                    b.HasKey("Id")
                        .HasName("pk_message");

                    b.HasIndex("AuthorId")
                        .HasDatabaseName("ix_message_author_id");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_message_channel_id");

                    b.HasIndex("ReceivedAt")
                        .HasDatabaseName("ix_message_received_at");

                    b.HasIndex("SourceId")
                        .HasDatabaseName("ix_message_source_id");

                    b.ToTable("message", "twitch");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.MessageSource", b =>
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
                        .HasName("pk_message_source");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_message_source_name");

                    b.ToTable("message_source", "twitch");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.ModeratorAction", b =>
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

                    b.Property<List<string>>("Args")
                        .HasColumnType("text[]")
                        .HasColumnName("args");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("MessageId")
                        .HasColumnType("text")
                        .HasColumnName("message_id");

                    b.Property<string>("ModeratorId")
                        .HasColumnType("text")
                        .HasColumnName("moderator_id");

                    b.Property<string>("ModeratorMessage")
                        .HasColumnType("text")
                        .HasColumnName("moderator_message");

                    b.Property<string>("TargetId")
                        .HasColumnType("text")
                        .HasColumnName("target_id");

                    b.Property<string>("TargetLogin")
                        .HasColumnType("text")
                        .HasColumnName("target_login");

                    b.HasKey("Id")
                        .HasName("pk_moderator_action");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_moderator_action_channel_id");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("ix_moderator_action_created_at");

                    b.HasIndex("ModeratorId")
                        .HasDatabaseName("ix_moderator_action_moderator_id");

                    b.HasIndex("TargetId")
                        .HasDatabaseName("ix_moderator_action_target_id");

                    b.ToTable("moderator_action", "twitch");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("FirstSeenAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("first_seen_at");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("Login")
                        .HasDatabaseName("ix_user_login");

                    b.ToTable("user", "twitch");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.ChatLog", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Channel")
                        .WithMany("ChatLogs")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chat_log_user_channel_id");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.PubSubLog", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Channel")
                        .WithMany("PubSubLogs")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_pubsub_log_user_channel_id");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.Message", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Author")
                        .WithMany("AuthorMessages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_message_user_author_id");

                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Channel")
                        .WithMany("ChannelMessages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_message_user_channel_id");

                    b.HasOne("TwitchLogger.Data.Models.Twitch.MessageSource", "Source")
                        .WithMany("Messages")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_message_message_sources_source_id");

                    b.Navigation("Author");

                    b.Navigation("Channel");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.ModeratorAction", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Channel")
                        .WithMany("ChannelModeratorActions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_moderator_action_user_channel_id");

                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Moderator")
                        .WithMany("ModeratorModeratorActions")
                        .HasForeignKey("ModeratorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_moderator_action_user_moderator_id");

                    b.HasOne("TwitchLogger.Data.Models.Twitch.User", "Target")
                        .WithMany("TargetModeratorActions")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_moderator_action_user_target_id");

                    b.Navigation("Channel");

                    b.Navigation("Moderator");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.MessageSource", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Twitch.User", b =>
                {
                    b.Navigation("AuthorMessages");

                    b.Navigation("ChannelMessages");

                    b.Navigation("ChannelModeratorActions");

                    b.Navigation("ChatLogs");

                    b.Navigation("ModeratorModeratorActions");

                    b.Navigation("PubSubLogs");

                    b.Navigation("TargetModeratorActions");
                });
#pragma warning restore 612, 618
        }
    }
}
