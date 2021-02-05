﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TwitchLogger.Data;

namespace TwitchLogger.Data.Migrations
{
    [DbContext(typeof(TwitchLoggerDbContext))]
    partial class TwitchLoggerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("TwitchLogger.Data.Models.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("AuthorId")
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

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ChannelId");

                    b.HasIndex("SourceId");

                    b.ToTable("message");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.MessageSource", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("message_source");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.ModeratorAction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

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

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("ModeratorId");

                    b.HasIndex("TargetId");

                    b.ToTable("moderator_action");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.User", b =>
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

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.Message", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.User", "Author")
                        .WithMany("Messages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TwitchLogger.Data.Models.User", "Channel")
                        .WithMany("ChannelMessages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TwitchLogger.Data.Models.MessageSource", "Source")
                        .WithMany("Messages")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Channel");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.ModeratorAction", b =>
                {
                    b.HasOne("TwitchLogger.Data.Models.User", "Channel")
                        .WithMany("ChannelModeratorActions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TwitchLogger.Data.Models.User", "Moderator")
                        .WithMany("ModeratorActionsIssued")
                        .HasForeignKey("ModeratorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TwitchLogger.Data.Models.User", "Target")
                        .WithMany("ModeratorActionsReceived")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Channel");

                    b.Navigation("Moderator");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.MessageSource", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("TwitchLogger.Data.Models.User", b =>
                {
                    b.Navigation("ChannelMessages");

                    b.Navigation("ChannelModeratorActions");

                    b.Navigation("Messages");

                    b.Navigation("ModeratorActionsIssued");

                    b.Navigation("ModeratorActionsReceived");
                });
#pragma warning restore 612, 618
        }
    }
}
