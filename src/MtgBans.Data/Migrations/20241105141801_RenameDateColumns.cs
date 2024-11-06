using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date",
                table: "format_event",
                newName: "date_effective");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "expansion_legality",
                newName: "date_entered");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "expansion_legality",
                newName: "date_exited");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "card_legality_event",
                newName: "date_effective");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "announcements",
                newName: "date_effective");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_effective",
                table: "format_event",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "date_exited",
                table: "expansion_legality",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "date_entered",
                table: "expansion_legality",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "date_effective",
                table: "card_legality_event",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "date_effective",
                table: "announcements",
                newName: "date");
        }
    }
}
