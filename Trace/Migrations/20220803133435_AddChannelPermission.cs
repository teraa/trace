using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    public partial class AddChannelPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channel_permissions",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_permissions", x => new { x.user_id, x.channel_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channel_permissions");
        }
    }
}
