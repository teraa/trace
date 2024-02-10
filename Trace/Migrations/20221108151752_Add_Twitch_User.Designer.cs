﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trace.Data;

#nullable disable

namespace Trace.Migrations
{
    [DbContext(typeof(TraceDbContext))]
    [Migration("20221108151752_Add_Twitch_User")]
    partial class Add_Twitch_User
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Trace.Data.Models.ChannelPermission", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.HasKey("UserId", "ChannelId")
                        .HasName("pk_channel_permissions");

                    b.ToTable("channel_permissions", (string)null);
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Config", b =>
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

            modelBuilder.Entity("Trace.Data.Models.Pubsub.ModeratorAction", b =>
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

            modelBuilder.Entity("Trace.Data.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<DateTimeOffset>("IssuedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("issued_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_refresh_tokens");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_refresh_tokens_user_id");

                    b.ToTable("refresh_tokens", (string)null);
                });

            modelBuilder.Entity("Trace.Data.Models.Tmi.Config", b =>
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

            modelBuilder.Entity("Trace.Data.Models.Tmi.Message", b =>
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

                    b.HasIndex("SourceId")
                        .HasDatabaseName("ix_messages_source_id");

                    b.HasIndex("Timestamp")
                        .HasDatabaseName("ix_messages_timestamp");

                    b.HasIndex("AuthorId", "Timestamp")
                        .HasDatabaseName("ix_messages_author_id_timestamp");

                    b.HasIndex("AuthorLogin", "Timestamp")
                        .HasDatabaseName("ix_messages_author_login_timestamp");

                    b.HasIndex("ChannelId", "AuthorId", "Timestamp")
                        .HasDatabaseName("ix_messages_channel_id_author_id_timestamp");

                    b.HasIndex("ChannelId", "AuthorLogin", "Timestamp")
                        .HasDatabaseName("ix_messages_channel_id_author_login_timestamp");

                    b.ToTable("messages", "tmi");
                });

            modelBuilder.Entity("Trace.Data.Models.Tmi.Source", b =>
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

            modelBuilder.Entity("Trace.Data.Models.Twitch.User", b =>
                {
                    b.Property<long>("EntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("entry_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("EntryId"));

                    b.Property<DateTimeOffset>("FirstSeen")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("first_seen");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("LastSeen")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_seen");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("EntryId")
                        .HasName("pk_users");

                    b.ToTable("users", "twitch");
                });

            modelBuilder.Entity("Trace.Data.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean")
                        .HasColumnName("is_verified");

                    b.Property<string>("TwitchId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("twitch_id");

                    b.Property<string>("TwitchLogin")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("twitch_login");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("TwitchId")
                        .IsUnique()
                        .HasDatabaseName("ix_users_twitch_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Followers", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.ModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration");

                    b.HasDiscriminator().HasValue("Followers");
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Raid", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.ModeratorAction");

                    b.Property<string>("TargetName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("target_name");

                    b.HasDiscriminator().HasValue("Raid");
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Slow", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.ModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("slow_duration");

                    b.HasDiscriminator().HasValue("Slow");
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.TargetedModeratorAction", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.ModeratorAction");

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

            modelBuilder.Entity("Trace.Data.Models.Pubsub.TermAction", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.ModeratorAction");

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

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Ban", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.HasDiscriminator().HasValue("Ban");
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Delete", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.TargetedModeratorAction");

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

            modelBuilder.Entity("Trace.Data.Models.Pubsub.Timeout", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("timeout_duration");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("timeout_reason");

                    b.HasDiscriminator().HasValue("Timeout");
                });

            modelBuilder.Entity("Trace.Data.Models.Pubsub.UnbanRequestAction", b =>
                {
                    b.HasBaseType("Trace.Data.Models.Pubsub.TargetedModeratorAction");

                    b.Property<string>("ModeratorMessage")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("moderator_message");

                    b.HasDiscriminator().HasValue("UnbanRequestAction");
                });

            modelBuilder.Entity("Trace.Data.Models.RefreshToken", b =>
                {
                    b.HasOne("Trace.Data.Models.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_refresh_tokens_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Trace.Data.Models.Tmi.Message", b =>
                {
                    b.HasOne("Trace.Data.Models.Tmi.Source", "Source")
                        .WithMany("Messages")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_messages_tmi_sources_source_id");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("Trace.Data.Models.Tmi.Source", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Trace.Data.Models.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
