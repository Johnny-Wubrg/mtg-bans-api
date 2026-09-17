using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCardLegalityRationaleVoteNonce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "card_legality_rationale_vote_nonces",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rationale_id = table.Column<int>(type: "integer", nullable: false),
                    date_issued = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_legality_rationale_vote_nonces", x => x.id);
                    table.ForeignKey(
                        name: "fk_card_legality_rationale_vote_nonces_card_legality_rationale",
                        column: x => x.rationale_id,
                        principalTable: "card_legality_rationale",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_vote_nonces_rationale_id",
                table: "card_legality_rationale_vote_nonces",
                column: "rationale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_legality_rationale_vote_nonces");
        }
    }
}
