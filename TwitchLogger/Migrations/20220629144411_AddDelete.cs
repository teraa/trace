using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    public partial class AddDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "message",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "message_id",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropColumn(
                name: "message_id",
                schema: "pubsub",
                table: "moderator_actions");
        }
    }
}
