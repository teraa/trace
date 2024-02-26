using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class Rename_LowTrustUser_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "low_user_trust_types",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_trust_user_types");

            migrationBuilder.RenameColumn(
                name: "low_user_trust_treatment",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_trust_user_treatment");

            migrationBuilder.RenameColumn(
                name: "low_user_trust_ban_evasion_evaluation",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_trust_user_ban_evasion_evaluation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "low_trust_user_types",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_user_trust_types");

            migrationBuilder.RenameColumn(
                name: "low_trust_user_treatment",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_user_trust_treatment");

            migrationBuilder.RenameColumn(
                name: "low_trust_user_ban_evasion_evaluation",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "low_user_trust_ban_evasion_evaluation");
        }
    }
}
