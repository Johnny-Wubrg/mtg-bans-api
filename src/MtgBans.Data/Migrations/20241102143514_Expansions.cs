using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class Expansions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "scryfall_uri",
                table: "cards",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "expansions",
                columns: table => new
                {
                    scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    scryfall_uri = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    date_released = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expansions", x => x.scryfall_id);
                });

            migrationBuilder.CreateTable(
                name: "expansion_legality",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    format_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expansion_scryfall_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expansion_legality", x => x.id);
                    table.ForeignKey(
                        name: "fk_expansion_legality_expansions_expansion_scryfall_id",
                        column: x => x.expansion_scryfall_id,
                        principalTable: "expansions",
                        principalColumn: "scryfall_id");
                    table.ForeignKey(
                        name: "fk_expansion_legality_formats_format_id",
                        column: x => x.format_id,
                        principalTable: "formats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_expansion_legality_expansion_scryfall_id",
                table: "expansion_legality",
                column: "expansion_scryfall_id");

            migrationBuilder.CreateIndex(
                name: "ix_expansion_legality_format_id",
                table: "expansion_legality",
                column: "format_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expansion_legality");

            migrationBuilder.DropTable(
                name: "expansions");

            migrationBuilder.AlterColumn<string>(
                name: "scryfall_uri",
                table: "cards",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
