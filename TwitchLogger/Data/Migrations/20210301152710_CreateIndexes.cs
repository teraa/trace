using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchLogger.Data.Migrations
{
    public partial class CreateIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_id",
                schema: "twitch",
                table: "user",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_login",
                schema: "twitch",
                table: "user",
                column: "login");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_action_created_at",
                schema: "twitch",
                table: "moderator_action",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_message_received_at",
                schema: "twitch",
                table: "message",
                column: "received_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_id",
                schema: "twitch",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_login",
                schema: "twitch",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_moderator_action_created_at",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropIndex(
                name: "IX_message_received_at",
                schema: "twitch",
                table: "message");
        }
    }
}
