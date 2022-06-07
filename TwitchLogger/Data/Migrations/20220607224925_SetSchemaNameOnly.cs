using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    public partial class SetSchemaNameOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_log_users_channel_id",
                table: "chat_log");

            migrationBuilder.DropForeignKey(
                name: "fk_message_message_sources_source_id",
                schema: "twitch",
                table: "message");

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

            migrationBuilder.RenameTable(
                name: "user",
                schema: "twitch",
                newName: "users",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "pubsub_log",
                newName: "pub_sub_logs");

            migrationBuilder.RenameTable(
                name: "moderator_action",
                schema: "twitch",
                newName: "moderator_actions",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "message_source",
                schema: "twitch",
                newName: "message_sources",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "message",
                schema: "twitch",
                newName: "messages",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "chat_log",
                newName: "chat_logs");

            migrationBuilder.RenameIndex(
                name: "ix_user_login",
                schema: "twitch",
                table: "users",
                newName: "ix_users_login");

            migrationBuilder.RenameIndex(
                name: "ix_pubsub_log_topic",
                table: "pub_sub_logs",
                newName: "ix_pub_sub_logs_topic");

            migrationBuilder.RenameIndex(
                name: "ix_pubsub_log_channel_id",
                table: "pub_sub_logs",
                newName: "ix_pub_sub_logs_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_target_id",
                schema: "twitch",
                table: "moderator_actions",
                newName: "ix_moderator_actions_target_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_moderator_id",
                schema: "twitch",
                table: "moderator_actions",
                newName: "ix_moderator_actions_moderator_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_created_at",
                schema: "twitch",
                table: "moderator_actions",
                newName: "ix_moderator_actions_created_at");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_action_channel_id",
                schema: "twitch",
                table: "moderator_actions",
                newName: "ix_moderator_actions_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_source_name",
                schema: "twitch",
                table: "message_sources",
                newName: "ix_message_sources_name");

            migrationBuilder.RenameIndex(
                name: "ix_message_source_id",
                schema: "twitch",
                table: "messages",
                newName: "ix_messages_source_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_received_at",
                schema: "twitch",
                table: "messages",
                newName: "ix_messages_received_at");

            migrationBuilder.RenameIndex(
                name: "ix_message_channel_id",
                schema: "twitch",
                table: "messages",
                newName: "ix_messages_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_author_id",
                schema: "twitch",
                table: "messages",
                newName: "ix_messages_author_id");

            migrationBuilder.RenameIndex(
                name: "ix_chat_log_channel_id",
                table: "chat_logs",
                newName: "ix_chat_logs_channel_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                schema: "twitch",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pub_sub_logs",
                table: "pub_sub_logs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_moderator_actions",
                schema: "twitch",
                table: "moderator_actions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message_sources",
                schema: "twitch",
                table: "message_sources",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_messages",
                schema: "twitch",
                table: "messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chat_logs",
                table: "chat_logs",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_logs_users_channel_id",
                table: "chat_logs",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_message_sources_source_id",
                schema: "twitch",
                table: "messages",
                column: "source_id",
                principalSchema: "twitch",
                principalTable: "message_sources",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_author_id",
                schema: "twitch",
                table: "messages",
                column: "author_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_channel_id",
                schema: "twitch",
                table: "messages",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_channel_id",
                schema: "twitch",
                table: "moderator_actions",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_moderator_id",
                schema: "twitch",
                table: "moderator_actions",
                column: "moderator_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_target_id",
                schema: "twitch",
                table: "moderator_actions",
                column: "target_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pub_sub_logs_users_channel_id",
                table: "pub_sub_logs",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_logs_users_channel_id",
                table: "chat_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_message_sources_source_id",
                schema: "twitch",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_author_id",
                schema: "twitch",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_channel_id",
                schema: "twitch",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_channel_id",
                schema: "twitch",
                table: "moderator_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_moderator_id",
                schema: "twitch",
                table: "moderator_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_target_id",
                schema: "twitch",
                table: "moderator_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_pub_sub_logs_users_channel_id",
                table: "pub_sub_logs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                schema: "twitch",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pub_sub_logs",
                table: "pub_sub_logs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_moderator_actions",
                schema: "twitch",
                table: "moderator_actions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_messages",
                schema: "twitch",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_message_sources",
                schema: "twitch",
                table: "message_sources");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chat_logs",
                table: "chat_logs");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "twitch",
                newName: "user",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "pub_sub_logs",
                newName: "pubsub_log");

            migrationBuilder.RenameTable(
                name: "moderator_actions",
                schema: "twitch",
                newName: "moderator_action",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "messages",
                schema: "twitch",
                newName: "message",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "message_sources",
                schema: "twitch",
                newName: "message_source",
                newSchema: "twitch");

            migrationBuilder.RenameTable(
                name: "chat_logs",
                newName: "chat_log");

            migrationBuilder.RenameIndex(
                name: "ix_users_login",
                schema: "twitch",
                table: "user",
                newName: "ix_user_login");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_logs_topic",
                table: "pubsub_log",
                newName: "ix_pubsub_log_topic");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_logs_channel_id",
                table: "pubsub_log",
                newName: "ix_pubsub_log_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_actions_target_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_target_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_actions_moderator_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_moderator_id");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_actions_created_at",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_created_at");

            migrationBuilder.RenameIndex(
                name: "ix_moderator_actions_channel_id",
                schema: "twitch",
                table: "moderator_action",
                newName: "ix_moderator_action_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_messages_source_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_source_id");

            migrationBuilder.RenameIndex(
                name: "ix_messages_received_at",
                schema: "twitch",
                table: "message",
                newName: "ix_message_received_at");

            migrationBuilder.RenameIndex(
                name: "ix_messages_channel_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_messages_author_id",
                schema: "twitch",
                table: "message",
                newName: "ix_message_author_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_sources_name",
                schema: "twitch",
                table: "message_source",
                newName: "ix_message_source_name");

            migrationBuilder.RenameIndex(
                name: "ix_chat_logs_channel_id",
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
                name: "pk_message",
                schema: "twitch",
                table: "message",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message_source",
                schema: "twitch",
                table: "message_source",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chat_log",
                table: "chat_log",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_log_users_channel_id",
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
    }
}
