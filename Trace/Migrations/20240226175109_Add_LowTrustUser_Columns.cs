using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class Add_LowTrustUser_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "low_user_trust_ban_evasion_evaluation",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "low_user_trust_treatment",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "low_user_trust_types",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "low_user_trust_ban_evasion_evaluation",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropColumn(
                name: "low_user_trust_treatment",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.DropColumn(
                name: "low_user_trust_types",
                schema: "pubsub",
                table: "moderator_actions");
        }
    }
}
