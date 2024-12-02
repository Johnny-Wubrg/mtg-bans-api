using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class DateAnnounced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_announced",
                table: "announcements",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
            
            // Copy values from the date_effective column to the date_announced column
            migrationBuilder.Sql("UPDATE announcements SET date_announced = date_effective");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_announced",
                table: "announcements");
        }
    }
}
