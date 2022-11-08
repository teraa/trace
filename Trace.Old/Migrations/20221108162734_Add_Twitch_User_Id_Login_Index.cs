using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    public partial class Add_Twitch_User_Id_Login_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_users_id",
                schema: "twitch",
                table: "users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_users_login",
                schema: "twitch",
                table: "users",
                column: "login");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_id",
                schema: "twitch",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_login",
                schema: "twitch",
                table: "users");
        }
    }
}
