using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardLegalityStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status_id",
                table: "card_legality_event",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "card_legality_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_legality_status", x => x.id);
                });
            
            migrationBuilder.Sql(@"
                INSERT INTO card_legality_status
                    (label, type, display_order)
                VALUES
                    ('Released', 0, 1),
                    ('Banned', 1, 2),
                    ('Restricted', 1, 3),
                    ('Unbanned', 0, 4),
                    ('Rotated', 0, 5),
                    ('Errata', 2, 6),
                    ('Banned as Commander', 1, 7),
                    ('Game Changers', 1, 8);
            ");
            
            migrationBuilder.Sql(@"
                UPDATE card_legality_event 
                SET status_id = case when type = 2 and format_id = 4 
                    then 
                        case when date_effective > '2025-01-01'
                            then 8
                            else 7
                        end
                    else type + 1
                end
            ");

            migrationBuilder.CreateIndex(
                name: "ix_card_legality_event_status_id",
                table: "card_legality_event",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_event_card_legality_status_status_id",
                table: "card_legality_event",
                column: "status_id",
                principalTable: "card_legality_status",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_event_card_legality_status_status_id",
                table: "card_legality_event");

            migrationBuilder.DropTable(
                name: "card_legality_status");

            migrationBuilder.DropIndex(
                name: "ix_card_legality_event_status_id",
                table: "card_legality_event");

            migrationBuilder.DropColumn(
                name: "status_id",
                table: "card_legality_event");
        }
    }
}
