using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    scryfall_image_url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cards", x => x.scryfall_id);
                });

            migrationBuilder.CreateTable(
                name: "formats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_formats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "card_legality_event",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    card_scryfall_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_legality_event", x => x.id);
                    table.ForeignKey(
                        name: "fk_card_legality_event_cards_card_scryfall_id",
                        column: x => x.card_scryfall_id,
                        principalTable: "cards",
                        principalColumn: "scryfall_id");
                });

            migrationBuilder.CreateTable(
                name: "format_event",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    format_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_format_event", x => x.id);
                    table.ForeignKey(
                        name: "fk_format_event_formats_format_id",
                        column: x => x.format_id,
                        principalTable: "formats",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_event_card_scryfall_id",
                table: "card_legality_event",
                column: "card_scryfall_id");

            migrationBuilder.CreateIndex(
                name: "ix_format_event_format_id",
                table: "format_event",
                column: "format_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_legality_event");

            migrationBuilder.DropTable(
                name: "format_event");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "formats");
        }
    }
}
