using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Trace.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pubsub");

            migrationBuilder.EnsureSchema(
                name: "tmi");

            migrationBuilder.CreateTable(
                name: "sources",
                schema: "tmi",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sources", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    first_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configs",
                schema: "pubsub",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_configs_users_channel_id",
                        column: x => x.channel_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "configs",
                schema: "tmi",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_configs_users_channel_id",
                        column: x => x.channel_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "tmi",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_id = table.Column<short>(type: "smallint", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<string>(type: "text", nullable: false),
                    author_login = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_tmi_sources_source_id",
                        column: x => x.source_id,
                        principalSchema: "tmi",
                        principalTable: "sources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_channel_id",
                        column: x => x.channel_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "moderator_actions",
                schema: "pubsub",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    initiator_id = table.Column<string>(type: "text", nullable: false),
                    initiator_name = table.Column<string>(type: "text", nullable: false),
                    discriminator = table.Column<string>(type: "text", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    target_name = table.Column<string>(type: "text", nullable: true),
                    slow_duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    target_id = table.Column<string>(type: "text", nullable: true),
                    targeted_moderator_action_target_name = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    timeout_duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    timeout_reason = table.Column<string>(type: "text", nullable: true),
                    moderator_message = table.Column<string>(type: "text", nullable: true),
                    term_id = table.Column<string>(type: "text", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moderator_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_moderator_actions_users_channel_id",
                        column: x => x.channel_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_moderator_actions_users_initiator_id",
                        column: x => x.initiator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_moderator_actions_users_target_id",
                        column: x => x.target_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_configs_channel_id",
                schema: "pubsub",
                table: "configs",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_configs_topic",
                schema: "pubsub",
                table: "configs",
                column: "topic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_configs_channel_id1",
                schema: "tmi",
                table: "configs",
                column: "channel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_messages_author_id",
                schema: "tmi",
                table: "messages",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_channel_id",
                schema: "tmi",
                table: "messages",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_source_id",
                schema: "tmi",
                table: "messages",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_timestamp",
                schema: "tmi",
                table: "messages",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_moderator_actions_action",
                schema: "pubsub",
                table: "moderator_actions",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "ix_moderator_actions_channel_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_moderator_actions_initiator_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "initiator_id");

            migrationBuilder.CreateIndex(
                name: "ix_moderator_actions_target_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "target_id");

            migrationBuilder.CreateIndex(
                name: "ix_moderator_actions_timestamp",
                schema: "pubsub",
                table: "moderator_actions",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_sources_name",
                schema: "tmi",
                table: "sources",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_login",
                table: "users",
                column: "login");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configs",
                schema: "pubsub");

            migrationBuilder.DropTable(
                name: "configs",
                schema: "tmi");

            migrationBuilder.DropTable(
                name: "messages",
                schema: "tmi");

            migrationBuilder.DropTable(
                name: "moderator_actions",
                schema: "pubsub");

            migrationBuilder.DropTable(
                name: "sources",
                schema: "tmi");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
