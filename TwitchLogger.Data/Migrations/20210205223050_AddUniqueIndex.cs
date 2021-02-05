using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchLogger.Data.Migrations
{
    public partial class AddUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log");

            migrationBuilder.CreateIndex(
                name: "IX_pubsub_log_topic",
                table: "pubsub_log",
                column: "topic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_source_name",
                schema: "twitch",
                table: "message_source",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log",
                column: "channel_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pubsub_log_topic",
                table: "pubsub_log");

            migrationBuilder.DropIndex(
                name: "IX_message_source_name",
                schema: "twitch",
                table: "message_source");

            migrationBuilder.DropIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log");

            migrationBuilder.CreateIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log",
                column: "channel_id");
        }
    }
}
