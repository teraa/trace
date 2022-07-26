using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Migrations
{
    public partial class UpdateTmiMessagesIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_messages_author_id",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_channel_id",
                schema: "tmi",
                table: "messages");

            migrationBuilder.CreateIndex(
                name: "ix_messages_author_id_timestamp",
                schema: "tmi",
                table: "messages",
                columns: new[] { "author_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_messages_author_login_timestamp",
                schema: "tmi",
                table: "messages",
                columns: new[] { "author_login", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_messages_channel_id_author_id_timestamp",
                schema: "tmi",
                table: "messages",
                columns: new[] { "channel_id", "author_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_messages_channel_id_author_login_timestamp",
                schema: "tmi",
                table: "messages",
                columns: new[] { "channel_id", "author_login", "timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_messages_author_id_timestamp",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_author_login_timestamp",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_channel_id_author_id_timestamp",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_channel_id_author_login_timestamp",
                schema: "tmi",
                table: "messages");

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
        }
    }
}
