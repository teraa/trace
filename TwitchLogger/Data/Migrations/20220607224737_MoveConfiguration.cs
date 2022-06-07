using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    public partial class MoveConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_log_user_channel_id",
                table: "chat_log");

            migrationBuilder.DropForeignKey(
                name: "fk_message_user_author_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "fk_message_user_channel_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_user_channel_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_user_moderator_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_user_target_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_pubsub_log_user_channel_id",
                table: "pubsub_log");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_log_users_channel_id",
                table: "chat_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_message_users_author_id",
                schema: "twitch",
                table: "message",
                column: "author_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_message_users_channel_id",
                schema: "twitch",
                table: "message",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_users_channel_id",
                schema: "twitch",
                table: "moderator_action",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_users_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                column: "moderator_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_users_target_id",
                schema: "twitch",
                table: "moderator_action",
                column: "target_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pubsub_log_users_channel_id",
                table: "pubsub_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_log_users_channel_id",
                table: "chat_log");

            migrationBuilder.DropForeignKey(
                name: "fk_message_users_author_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "fk_message_users_channel_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_users_channel_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_users_moderator_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_action_users_target_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "fk_pubsub_log_users_channel_id",
                table: "pubsub_log");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_log_user_channel_id",
                table: "chat_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_message_user_author_id",
                schema: "twitch",
                table: "message",
                column: "author_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_message_user_channel_id",
                schema: "twitch",
                table: "message",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_user_channel_id",
                schema: "twitch",
                table: "moderator_action",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_user_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                column: "moderator_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_action_user_target_id",
                schema: "twitch",
                table: "moderator_action",
                column: "target_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pubsub_log_user_channel_id",
                table: "pubsub_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
