using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class FormatEventAnnouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "announcement_id",
                table: "format_event",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_format_event_announcement_id",
                table: "format_event",
                column: "announcement_id");

            migrationBuilder.AddForeignKey(
                name: "fk_format_event_announcements_announcement_id",
                table: "format_event",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_format_event_announcements_announcement_id",
                table: "format_event");

            migrationBuilder.DropIndex(
                name: "ix_format_event_announcement_id",
                table: "format_event");

            migrationBuilder.DropColumn(
                name: "announcement_id",
                table: "format_event");
        }
    }
}
