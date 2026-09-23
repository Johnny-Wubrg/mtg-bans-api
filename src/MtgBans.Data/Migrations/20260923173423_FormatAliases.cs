using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class FormatAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "aliases",
                table: "formats",
                type: "text[]",
                nullable: true);

            migrationBuilder.Sql(@"
            UPDATE formats f
            SET aliases = COALESCE(
                (
                    SELECT array_agg(d.name_update ORDER BY d.first_effective, d.name_update)
                    FROM (
                        SELECT fe.name_update, MIN(fe.date_effective) AS first_effective
                        FROM format_event fe
                        WHERE fe.format_id = f.id
                          AND fe.name_update IS NOT NULL
                          AND fe.name_update <> f.name
                        GROUP BY fe.name_update
                    ) d
                ),
                '{}'
            );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aliases",
                table: "formats");
        }
    }
}
