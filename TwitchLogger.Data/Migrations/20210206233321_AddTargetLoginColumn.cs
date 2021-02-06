using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchLogger.Data.Migrations
{
    public partial class AddTargetLoginColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "target_login",
                schema: "twitch",
                table: "moderator_action",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "target_login",
                schema: "twitch",
                table: "moderator_action");
        }
    }
}
