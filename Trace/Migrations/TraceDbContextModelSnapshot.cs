﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trace.Data;

#nullable disable

namespace Trace.Migrations
{
    [DbContext(typeof(TraceDbContext))]
    partial class TraceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
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

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration");

                    b.Property<string>("InitiatorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("initiator_id");

                    b.Property<string>("InitiatorName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("initiator_name");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("MessageId")
                        .HasColumnType("text")
                        .HasColumnName("message_id");

                    b.Property<string>("ModeratorMessage")
                        .HasColumnType("text")
                        .HasColumnName("moderator_message");

                    b.Property<string>("Reason")
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<string>("TargetId")
                        .HasColumnType("text")
                        .HasColumnName("target_id");

                    b.Property<string>("TargetName")
                        .HasColumnType("text")
                        .HasColumnName("target_name");

                    b.Property<string>("TermId")
                        .HasColumnType("text")
                        .HasColumnName("term_id");

                    b.Property<string>("TermText")
                        .HasColumnType("text")
                        .HasColumnName("term_text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_moderator_actions");

                    b.HasIndex("Action")
                        .HasDatabaseName("ix_moderator_actions_action");

                    b.HasIndex("ChannelId")
                        .HasDatabaseName("ix_moderator_actions_channel_id");

                    b.HasIndex("InitiatorId")
                        .HasDatabaseName("ix_moderator_actions_initiator_id");

                    b.HasIndex("TargetId")
                        .HasDatabaseName("ix_moderator_actions_target_id");

                    b.HasIndex("Timestamp")
                        .HasDatabaseName("ix_moderator_actions_timestamp");

                    b.ToTable("moderator_actions", "pubsub");
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

                    b.HasIndex("ChannelId", "Timestamp")
                        .HasDatabaseName("ix_messages_channel_id_timestamp");

                    b.HasIndex("ChannelId", "AuthorId", "Timestamp")
                        .HasDatabaseName("ix_messages_channel_id_author_id_timestamp");

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

                    b.HasIndex("Id")
                        .HasDatabaseName("ix_users_id");

                    b.HasIndex("Login")
                        .HasDatabaseName("ix_users_login");

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
