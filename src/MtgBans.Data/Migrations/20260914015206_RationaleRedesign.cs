using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class RationaleRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_rationale_vote_card_legality_rationale_card_s",
                table: "card_legality_rationale_vote");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_rationale_vote_card_scryfall_id",
                table: "card_legality_rationale_vote");

            migrationBuilder.DropPrimaryKey(
                name: "pk_card_legality_rationale",
                table: "card_legality_rationale");

            migrationBuilder.DropColumn(
                name: "card_scryfall_id",
                table: "card_legality_rationale_vote");

            migrationBuilder.AddColumn<int>(
                name: "rationale_id",
                table: "card_legality_rationale_vote",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "card_legality_rationale",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ai_model",
                table: "card_legality_rationale",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_approved",
                table: "card_legality_rationale",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "format_id",
                table: "card_legality_rationale",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_card_legality_rationale",
                table: "card_legality_rationale",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_vote_rationale_id",
                table: "card_legality_rationale_vote",
                column: "rationale_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_card_scryfall_id",
                table: "card_legality_rationale",
                column: "card_scryfall_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_card_scryfall_id_format_id",
                table: "card_legality_rationale",
                columns: new[] { "card_scryfall_id", "format_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_format_id",
                table: "card_legality_rationale",
                column: "format_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_rationale_formats_format_id",
                table: "card_legality_rationale",
                column: "format_id",
                principalTable: "formats",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_rationale_vote_card_legality_rationale_ration",
                table: "card_legality_rationale_vote",
                column: "rationale_id",
                principalTable: "card_legality_rationale",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_rationale_formats_format_id",
                table: "card_legality_rationale");

            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_rationale_vote_card_legality_rationale_ration",
                table: "card_legality_rationale_vote");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_rationale_vote_rationale_id",
                table: "card_legality_rationale_vote");

            migrationBuilder.DropPrimaryKey(
                name: "pk_card_legality_rationale",
                table: "card_legality_rationale");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_rationale_card_scryfall_id",
                table: "card_legality_rationale");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_rationale_card_scryfall_id_format_id",
                table: "card_legality_rationale");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_rationale_format_id",
                table: "card_legality_rationale");

            migrationBuilder.DropColumn(
                name: "rationale_id",
                table: "card_legality_rationale_vote");

            migrationBuilder.DropColumn(
                name: "id",
                table: "card_legality_rationale");

            migrationBuilder.DropColumn(
                name: "ai_model",
                table: "card_legality_rationale");

            migrationBuilder.DropColumn(
                name: "date_approved",
                table: "card_legality_rationale");

            migrationBuilder.DropColumn(
                name: "format_id",
                table: "card_legality_rationale");

            migrationBuilder.AddColumn<Guid>(
                name: "card_scryfall_id",
                table: "card_legality_rationale_vote",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_card_legality_rationale",
                table: "card_legality_rationale",
                column: "card_scryfall_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_rationale_vote_card_scryfall_id",
                table: "card_legality_rationale_vote",
                column: "card_scryfall_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_rationale_vote_card_legality_rationale_card_s",
                table: "card_legality_rationale_vote",
                column: "card_scryfall_id",
                principalTable: "card_legality_rationale",
                principalColumn: "card_scryfall_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
