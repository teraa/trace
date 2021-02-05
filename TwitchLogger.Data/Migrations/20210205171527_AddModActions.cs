using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TwitchLogger.Data.Migrations
{
    public partial class AddModActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "moderator_action",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    args = table.Column<List<string>>(type: "text[]", nullable: true),
                    message_id = table.Column<string>(type: "text", nullable: true),
                    moderator_id = table.Column<string>(type: "text", nullable: true),
                    target_id = table.Column<string>(type: "text", nullable: true),
                    moderator_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderator_action", x => x.id);
                    table.ForeignKey(
                        name: "FK_moderator_action_user_channel_id",
                        column: x => x.channel_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_moderator_action_user_moderator_id",
                        column: x => x.moderator_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_moderator_action_user_target_id",
                        column: x => x.target_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_moderator_action_channel_id",
                table: "moderator_action",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_action_moderator_id",
                table: "moderator_action",
                column: "moderator_id");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_action_target_id",
                table: "moderator_action",
                column: "target_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderator_action");
        }
    }
}
