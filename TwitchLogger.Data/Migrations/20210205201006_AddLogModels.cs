using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TwitchLogger.Data.Migrations
{
    public partial class AddLogModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_chat_log_user_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "twitch",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pubsub_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pubsub_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_pubsub_log_user_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "twitch",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_log_channel_id",
                table: "chat_log",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_pubsub_log_channel_id",
                table: "pubsub_log",
                column: "channel_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_log");

            migrationBuilder.DropTable(
                name: "pubsub_log");
        }
    }
}
