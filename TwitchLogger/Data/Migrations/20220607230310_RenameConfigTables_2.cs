using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    public partial class RenameConfigTables_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_logs_users_channel_id",
                table: "chat_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_pub_sub_logs_users_channel_id",
                table: "pub_sub_logs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pub_sub_logs",
                table: "pub_sub_logs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chat_logs",
                table: "chat_logs");

            migrationBuilder.RenameTable(
                name: "pub_sub_logs",
                newName: "pub_sub_configs");

            migrationBuilder.RenameTable(
                name: "chat_logs",
                newName: "tmi_configs");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_logs_topic",
                table: "pub_sub_configs",
                newName: "ix_pub_sub_configs_topic");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_logs_channel_id",
                table: "pub_sub_configs",
                newName: "ix_pub_sub_configs_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_chat_logs_channel_id",
                table: "tmi_configs",
                newName: "ix_tmi_configs_channel_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pub_sub_configs",
                table: "pub_sub_configs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tmi_configs",
                table: "tmi_configs",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_pub_sub_configs_users_channel_id",
                table: "pub_sub_configs",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tmi_configs_users_channel_id",
                table: "tmi_configs",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pub_sub_configs_users_channel_id",
                table: "pub_sub_configs");

            migrationBuilder.DropForeignKey(
                name: "fk_tmi_configs_users_channel_id",
                table: "tmi_configs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tmi_configs",
                table: "tmi_configs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pub_sub_configs",
                table: "pub_sub_configs");

            migrationBuilder.RenameTable(
                name: "tmi_configs",
                newName: "chat_logs");

            migrationBuilder.RenameTable(
                name: "pub_sub_configs",
                newName: "pub_sub_logs");

            migrationBuilder.RenameIndex(
                name: "ix_tmi_configs_channel_id",
                table: "chat_logs",
                newName: "ix_chat_logs_channel_id");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_configs_topic",
                table: "pub_sub_logs",
                newName: "ix_pub_sub_logs_topic");

            migrationBuilder.RenameIndex(
                name: "ix_pub_sub_configs_channel_id",
                table: "pub_sub_logs",
                newName: "ix_pub_sub_logs_channel_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chat_logs",
                table: "chat_logs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pub_sub_logs",
                table: "pub_sub_logs",
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
                name: "fk_pub_sub_logs_users_channel_id",
                table: "pub_sub_logs",
                column: "channel_id",
                principalSchema: "twitch",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
