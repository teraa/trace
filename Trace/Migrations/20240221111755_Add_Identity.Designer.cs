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
    [DbContext(typeof(AppDbContext))]
    [Migration("20240221111755_Add_Identity")]
    partial class Add_Identity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("ix_asp_net_roles_normalized_name");

                    b.ToTable("asp_net_roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_asp_net_role_claims_role_id");

                    b.ToTable("asp_net_role_claims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_asp_net_user_claims_user_id");

                    b.ToTable("asp_net_user_claims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_asp_net_user_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_asp_net_user_logins_user_id");

                    b.ToTable("asp_net_user_logins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("RoleId")
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_asp_net_user_roles");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_asp_net_user_roles_role_id");

                    b.ToTable("asp_net_user_roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_asp_net_user_tokens");

                    b.ToTable("asp_net_user_tokens", (string)null);
                });

            modelBuilder.Entity("Trace.Data.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("ix_asp_net_users_normalized_email");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("ix_asp_net_users_normalized_user_name");

                    b.ToTable("asp_net_users", (string)null);
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_role_claims_asp_net_roles_role_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Trace.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_claims_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Trace.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_logins_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_roles_role_id");

                    b.HasOne("Trace.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Trace.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_tokens_asp_net_users_user_id");
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
#pragma warning restore 612, 618
        }
    }
}