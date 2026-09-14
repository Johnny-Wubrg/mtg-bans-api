using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class NextProjectedAnnouncementDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_next_projected",
                table: "announcements",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_next_projected",
                table: "announcements");
        }
    }
}
