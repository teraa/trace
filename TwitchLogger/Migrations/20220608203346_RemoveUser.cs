using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchLogger.Migrations
{
    public partial class RemoveUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    first_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_login",
                table: "users",
                column: "login");
        }
    }
}
