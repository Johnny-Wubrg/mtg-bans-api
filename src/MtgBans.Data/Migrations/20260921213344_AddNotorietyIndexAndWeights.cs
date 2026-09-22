using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotorietyIndexAndWeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "notoriety_weight",
                table: "classification",
                type: "numeric(3,2)",
                nullable: false,
                defaultValue: 0.15m);

            migrationBuilder.CreateTable(
                name: "card_notoriety_indices",
                columns: table => new
                {
                    card_scryfall_id = table.Column<Guid>(type: "uuid", nullable: false),
                    index_value = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    computed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_notoriety_indices", x => x.card_scryfall_id);
                    table.ForeignKey(
                        name: "fk_card_notoriety_indices_cards_card_scryfall_id",
                        column: x => x.card_scryfall_id,
                        principalTable: "cards",
                        principalColumn: "scryfall_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "format_status_weight",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    format_id = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_format_status_weight", x => x.id);
                    table.ForeignKey(
                        name: "fk_format_status_weight_card_legality_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "card_legality_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_format_status_weight_formats_format_id",
                        column: x => x.format_id,
                        principalTable: "formats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_format_status_weight_format_id_status_id",
                table: "format_status_weight",
                columns: new[] { "format_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_format_status_weight_status_id",
                table: "format_status_weight",
                column: "status_id");

            // Seed one row per (format, status) combination that's meaningful for that format, at a
            // flat 1.0 placeholder weight. Real values are set by hand through the publisher's new
            // Formats admin section.
            migrationBuilder.Sql(@"
                INSERT INTO format_status_weight (format_id, status_id, weight)
                SELECT f.id, s.id, 1.0
                FROM (VALUES
                    ('vintage', 'Banned'), ('vintage', 'Restricted'),
                    ('legacy', 'Banned'), ('legacy', 'Restricted'),
                    ('modern', 'Banned'), ('modern', 'Restricted'),
                    ('commander', 'Banned'), ('commander', 'Banned as Commander'), ('commander', 'Game Changers'),
                    ('pioneer', 'Banned'), ('pioneer', 'Restricted'),
                    ('pauper', 'Banned'), ('pauper', 'Restricted'),
                    ('standard', 'Banned'), ('standard', 'Restricted'),
                    ('extended', 'Banned')
                ) AS v(slug, label)
                JOIN formats f ON f.slug = v.slug
                JOIN card_legality_statuses s ON s.label = v.label;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_notoriety_indices");

            migrationBuilder.DropTable(
                name: "format_status_weight");

            migrationBuilder.DropColumn(
                name: "notoriety_weight",
                table: "classification");
        }
    }
}
