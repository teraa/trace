using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    public partial class AddSnakeCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_log_user_channel_id",
                table: "chat_log");

            migrationBuilder.DropForeignKey(
                name: "FK_message_message_source_source_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "FK_message_user_author_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "FK_message_user_channel_id",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "FK_moderator_action_user_channel_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "FK_moderator_action_user_moderator_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "FK_moderator_action_user_target_id",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropForeignKey(
                name: "FK_pubsub_log_user_channel_id",
                table: "pubsub_log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                schema: "twitch",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pubsub_log",
                table: "pubsub_log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moderator_action",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message_source",
                schema: "twitch",
                table: "message_source");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chat_log",
                table: "chat_log");

            migrationBuilder.RenameIndex(
                name: "IX_user_login",
                schema: "twitch",
                table: "user",
                newName: "ix_user_login");

            migrationBuilder.RenameIndex(
                name: "IX_user_id",
                schema: "twitch",
                table: "user",
                newName: "ix_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_pubsub_log_topic",
                table: "pubsub_log",
                newName: "ix_pubsub_log_topic");

            migrationBuilder.RenameIndex(
                name: "IX_pubsub_log_channel_id",
                table: "pubsub_log",
                newName: "ix_pubsub_log_channel_id");

            migrationBuilder.RenameIndex(
                name: "IX_moderator_action_target_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_target_id");

            migrationBuilder.RenameIndex(
                name: "IX_moderator_action_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_moderator_id");

            migrationBuilder.RenameIndex(
                name: "IX_moderator_action_created_at",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_moderator_action_channel_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_channel_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_source_name",
                schema: "twitch",
                table: "message_source",
                newName: "ix_message_source_name");

            migrationBuilder.RenameIndex(
                name: "IX_message_source_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_source_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_received_at",
                schema: "twitch",
                table: "message",
                newName: "ix_message_received_at");

            migrationBuilder.RenameIndex(
                name: "IX_message_channel_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_channel_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_author_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_author_id");

            migrationBuilder.RenameIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log",
                newName: "ix_chat_log_channel_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                schema: "twitch",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pubsub_log",
                table: "pubsub_log",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_moderator_action",
                schema: "twitch",
                table: "moderator_action",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message_source",
                schema: "twitch",
                table: "message_source",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message",
                schema: "twitch",
                table: "message",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chat_log",
                table: "chat_log",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_log_user_channel_id",
                table: "chat_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_message_message_sources_source_id",
                schema: "twitch",
                table: "message",
                column: "source_id",
                principalSchema: "twitch",
                principalTable: "message_source",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_log_user_channel_id",
                table: "chat_log");

            migrationBuilder.DropForeignKey(
                name: "fk_message_message_sources_source_id",
                schema: "twitch",
                table: "message");

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

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                schema: "twitch",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pubsub_log",
                table: "pubsub_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_moderator_action",
                schema: "twitch",
                table: "moderator_action");

            migrationBuilder.DropPrimaryKey(
                name: "pk_message_source",
                schema: "twitch",
                table: "message_source");

            migrationBuilder.DropPrimaryKey(
                name: "pk_message",
                schema: "twitch",
                table: "message");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chat_log",
                table: "chat_log");

            migrationBuilder.RenameIndex(
                name: "ix_user_login",
                schema: "twitch",
                table: "user",
                newName: "IX_user_login");

            migrationBuilder.RenameIndex(
                name: "ix_user_id",
                schema: "twitch",
                table: "user",
                newName: "IX_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_pubsub_log_topic",
                table: "pubsub_log",
                newName: "IX_pubsub_log_topic");

            migrationBuilder.RenameIndex(
                name: "ix_pubsub_log_channel_id",
                table: "pubsub_log",
                newName: "IX_pubsub_log_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_target_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "IX_moderator_action_target_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "IX_moderator_action_moderator_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_created_at",
                schema: "twitch",
                table: "moderator_action",
                newName: "IX_moderator_action_created_at");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_channel_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "IX_moderator_action_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_source_name",
                schema: "twitch",
                table: "message_source",
                newName: "IX_message_source_name");

            migrationBuilder.RenameIndex(
                name: "ix_message_source_id",
                schema: "twitch",
                table: "message",
                newName: "IX_message_source_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_received_at",
                schema: "twitch",
                table: "message",
                newName: "IX_message_received_at");

            migrationBuilder.RenameIndex(
                name: "ix_message_channel_id",
                schema: "twitch",
                table: "message",
                newName: "IX_message_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_author_id",
                schema: "twitch",
                table: "message",
                newName: "IX_message_author_id");

            migrationBuilder.RenameIndex(
                name: "ix_chat_log_channel_id",
                table: "chat_log",
                newName: "IX_chat_log_channel_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                schema: "twitch",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pubsub_log",
                table: "pubsub_log",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moderator_action",
                schema: "twitch",
                table: "moderator_action",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message_source",
                schema: "twitch",
                table: "message_source",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message",
                schema: "twitch",
                table: "message",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_chat_log",
                table: "chat_log",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_chat_log_user_channel_id",
                table: "chat_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_message_message_source_source_id",
                schema: "twitch",
                table: "message",
                column: "source_id",
                principalSchema: "twitch",
                principalTable: "message_source",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_author_id",
                schema: "twitch",
                table: "message",
                column: "author_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_channel_id",
                schema: "twitch",
                table: "message",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_moderator_action_user_channel_id",
                schema: "twitch",
                table: "moderator_action",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_moderator_action_user_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                column: "moderator_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_moderator_action_user_target_id",
                schema: "twitch",
                table: "moderator_action",
                column: "target_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pubsub_log_user_channel_id",
                table: "pubsub_log",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
