using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Data.Migrations
{
    public partial class RemoveUserPkIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_id",
                schema: "twitch",
                table: "user");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_user_id",
                schema: "twitch",
                table: "user",
                column: "id");
        }
    }
}
