using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CanonicalPrinting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scryfall_image_uri",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "scryfall_uri",
                table: "cards");

            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uri",
                table: "printings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "scryfall_uri",
                table: "printings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "canonical_id",
                table: "cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cards_canonical_id",
                table: "cards",
                column: "canonical_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cards_printings_canonical_id",
                table: "cards",
                column: "canonical_id",
                principalTable: "printings",
                principalColumn: "scryfall_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cards_printings_canonical_id",
                table: "cards");

            migrationBuilder.DropIndex(
                name: "ix_cards_canonical_id",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "scryfall_image_uri",
                table: "printings");

            migrationBuilder.DropColumn(
                name: "scryfall_uri",
                table: "printings");

            migrationBuilder.DropColumn(
                name: "canonical_id",
                table: "cards");

            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uri",
                table: "cards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "scryfall_uri",
                table: "cards",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
