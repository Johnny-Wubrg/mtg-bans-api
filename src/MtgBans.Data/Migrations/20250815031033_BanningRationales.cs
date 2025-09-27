using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class BanningRationales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "card_legality_rationale",
                columns: table => new
                {
                    card_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: true),
                    ai_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_legality_rationale", x => x.card_scryfall_id);
                    table.ForeignKey(
                        name: "fk_card_legality_rationale_cards_card_scryfall_id",
                        column: x => x.card_scryfall_id,
                        principalTable: "cards",
                        principalColumn: "scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "card_legality_rationale_vote",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    card_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direction = table.Column<short>(type: "smallint", nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_legality_rationale_vote", x => x.id);
                    table.ForeignKey(
                        name: "fk_card_legality_rationale_vote_card_legality_rationale_card_s",
                        column: x => x.card_scryfall_id,
                        principalTable: "card_legality_rationale",
                        principalColumn: "card_scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_vote_card_scryfall_id",
                table: "card_legality_rationale_vote",
                column: "card_scryfall_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_legality_rationale_vote");

            migrationBuilder.DropTable(
                name: "card_legality_rationale");
        }
    }
}
