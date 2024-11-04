using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardPrints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "printings",
                columns: table => new
                {
                    scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expansion_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_printings", x => x.scryfall_id);
                    table.ForeignKey(
                        name: "fk_printings_cards_card_scryfall_id",
                        column: x => x.card_scryfall_id,
                        principalTable: "cards",
                        principalColumn: "scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_printings_expansions_expansion_scryfall_id",
                        column: x => x.expansion_scryfall_id,
                        principalTable: "expansions",
                        principalColumn: "scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_printings_card_scryfall_id",
                table: "printings",
                column: "card_scryfall_id");

            migrationBuilder.CreateIndex(
                name: "ix_printings_expansion_scryfall_id",
                table: "printings",
                column: "expansion_scryfall_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "printings");
        }
    }
}
