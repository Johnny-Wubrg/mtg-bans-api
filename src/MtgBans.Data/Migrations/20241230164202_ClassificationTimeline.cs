using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClassificationTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_applied",
                table: "classification",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "date_lifted",
                table: "classification",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "card_classification",
                columns: table => new
                {
                    cards_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    classifications_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_classification", x => new { x.cards_scryfall_id, x.classifications_id });
                    table.ForeignKey(
                        name: "fk_card_classification_cards_cards_scryfall_id",
                        column: x => x.cards_scryfall_id,
                        principalTable: "cards",
                        principalColumn: "scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_card_classification_classification_classifications_id",
                        column: x => x.classifications_id,
                        principalTable: "classification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_classification_classifications_id",
                table: "card_classification",
                column: "classifications_id");

            migrationBuilder.Sql("""
                insert into card_classification
                        (cards_scryfall_id, classifications_id) 
                select scryfall_id, classification_id
                from cards
                where classification_id is not null
                """);
            
            migrationBuilder.DropForeignKey(
                name: "fk_cards_classification_classification_id",
                table: "cards");

            migrationBuilder.DropIndex(
                name: "ix_cards_classification_id",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "classification_id",
                table: "cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "classification_id",
                table: "cards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cards_classification_id",
                table: "cards",
                column: "classification_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cards_classification_classification_id",
                table: "cards",
                column: "classification_id",
                principalTable: "classification",
                principalColumn: "id");

            migrationBuilder.Sql("""
                                 with classification as (
                                     select cards_scryfall_id, min(l.classifications_id) as classifications_id
                                     from card_classification l
                                     group by l.cards_scryfall_id
                                 )
                                 update cards
                                 set classification_id = l.classifications_id
                                 from classification l where l.cards_scryfall_id=cards.scryfall_id
                                 """);
            
            migrationBuilder.DropTable(
                name: "card_classification");

            migrationBuilder.DropColumn(
                name: "date_applied",
                table: "classification");

            migrationBuilder.DropColumn(
                name: "date_lifted",
                table: "classification");
        }
    }
}
