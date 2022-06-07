using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchLogger.Data.Migrations
{
    public partial class CreateTwitchSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "twitch");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "user",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "moderator_action",
                newName: "moderator_action",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "message_source",
                newName: "message_source",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "message",
                newName: "message",
                newSchema: "twitch");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "user",
                schema: "twitch",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "moderator_action",
                schema: "twitch",
                newName: "moderator_action");

            migrationBuilder.RenameTable(
                name: "message_source",
                schema: "twitch",
                newName: "message_source");

            migrationBuilder.RenameTable(
                name: "message",
                schema: "twitch",
                newName: "message");
        }
    }
}
