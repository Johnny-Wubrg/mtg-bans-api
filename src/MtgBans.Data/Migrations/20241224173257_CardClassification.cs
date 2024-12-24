using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "classification_id",
                table: "cards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "classification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_classification", x => x.id);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cards_classification_classification_id",
                table: "cards");

            migrationBuilder.DropTable(
                name: "classification");

            migrationBuilder.DropIndex(
                name: "ix_cards_classification_id",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "classification_id",
                table: "cards");
        }
    }
}
