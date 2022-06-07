using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchLogger.Data.Migrations
{
    public partial class RenameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_user_id",
                table: "message");

            migrationBuilder.RenameColumn(
                name: "user_login",
                table: "message",
                newName: "author_login");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "message",
                newName: "author_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_user_id",
                table: "message",
                newName: "IX_message_author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_author_id",
                table: "message",
                column: "author_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_author_id",
                table: "message");

            migrationBuilder.RenameColumn(
                name: "author_login",
                table: "message",
                newName: "user_login");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "message",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_author_id",
                table: "message",
                newName: "IX_message_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_user_id",
                table: "message",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
