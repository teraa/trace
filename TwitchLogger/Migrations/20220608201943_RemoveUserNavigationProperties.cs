using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    public partial class RemoveUserNavigationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_configs_users_channel_id",
                schema: "pubsub",
                table: "configs");

            migrationBuilder.DropForeignKey(
                name: "fk_configs_users_channel_id",
                schema: "tmi",
                table: "configs");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_author_id",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_channel_id",
                schema: "tmi",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_channel_id",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_initiator_id",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_moderator_actions_users_target_id",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropIndex(
                name: "ix_configs_channel_id",
                schema: "pubsub",
                table: "configs");

            migrationBuilder.RenameIndex(
                name: "ix_configs_channel_id1",
                schema: "tmi",
                table: "configs",
                newName: "ix_configs_channel_id");

            migrationBuilder.AddColumn<string>(
                name: "channel_login",
                schema: "tmi",
                table: "configs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "channel_login",
                schema: "tmi",
                table: "configs");

            migrationBuilder.RenameIndex(
                name: "ix_configs_channel_id",
                schema: "tmi",
                table: "configs",
                newName: "ix_configs_channel_id1");

            migrationBuilder.CreateIndex(
                name: "ix_configs_channel_id",
                schema: "pubsub",
                table: "configs",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_configs_users_channel_id",
                schema: "pubsub",
                table: "configs",
                column: "channel_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_configs_users_channel_id",
                schema: "tmi",
                table: "configs",
                column: "channel_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_author_id",
                schema: "tmi",
                table: "messages",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_channel_id",
                schema: "tmi",
                table: "messages",
                column: "channel_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_channel_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "channel_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_initiator_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "initiator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_actions_users_target_id",
                schema: "pubsub",
                table: "moderator_actions",
                column: "target_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
