using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class Announcements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "scryfall_image_url",
                table: "cards",
                newName: "scryfall_image_uri");

            migrationBuilder.AddColumn<string>(
                name: "scryfall_uri",
                table: "cards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "announcement_id",
                table: "card_legality_event",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "format_id",
                table: "card_legality_event",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    uri = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcements", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_event_announcement_id",
                table: "card_legality_event",
                column: "announcement_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_event_format_id",
                table: "card_legality_event",
                column: "format_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_event_announcements_announcement_id",
                table: "card_legality_event",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_event_formats_format_id",
                table: "card_legality_event",
                column: "format_id",
                principalTable: "formats",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_event_announcements_announcement_id",
                table: "card_legality_event");

            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_event_formats_format_id",
                table: "card_legality_event");

            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_event_announcement_id",
                table: "card_legality_event");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_event_format_id",
                table: "card_legality_event");

            migrationBuilder.DropColumn(
                name: "scryfall_uri",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "announcement_id",
                table: "card_legality_event");

            migrationBuilder.DropColumn(
                name: "format_id",
                table: "card_legality_event");

            migrationBuilder.RenameColumn(
                name: "scryfall_image_uri",
                table: "cards",
                newName: "scryfall_image_url");
        }
    }
}
