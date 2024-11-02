using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "uri",
                table: "announcements");

            migrationBuilder.AddColumn<string>(
                name: "name_update",
                table: "format_event",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "sources",
                table: "announcements",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name_update",
                table: "format_event");

            migrationBuilder.DropColumn(
                name: "sources",
                table: "announcements");

            migrationBuilder.AddColumn<string>(
                name: "uri",
                table: "announcements",
                type: "text",
                nullable: true);
        }
    }
}
