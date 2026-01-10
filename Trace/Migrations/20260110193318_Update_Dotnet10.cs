using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class Update_Dotnet10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_configs",
                schema: "tmi",
                table: "configs");

            migrationBuilder.AddPrimaryKey(
                name: "pk_configs1",
                schema: "tmi",
                table: "configs",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_configs1",
                schema: "tmi",
                table: "configs");

            migrationBuilder.AddPrimaryKey(
                name: "pk_configs",
                schema: "tmi",
                table: "configs",
                column: "id");
        }
    }
}
