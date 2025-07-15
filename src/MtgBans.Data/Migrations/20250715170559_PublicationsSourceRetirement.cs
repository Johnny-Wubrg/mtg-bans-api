using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class PublicationsSourceRetirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old sources column
            migrationBuilder.DropColumn(
                name: "sources",
                table: "announcements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back the sources column
            migrationBuilder.AddColumn<string[]>(
                name: "sources",
                table: "announcements",
                type: "text[]",
                nullable: true);

            // Migrate data back from publications to sources array
            migrationBuilder.Sql(@"
                -- Populate sources array from publications relationships
                UPDATE announcements 
                SET sources = subquery.source_array
                FROM (
                    SELECT 
                        a.id,
                        array_agg(p.uri) as source_array
                    FROM announcements a
                    JOIN announcement_publication ap ON a.id = ap.announcements_id
                    JOIN publications p ON ap.sources_id = p.id
                    GROUP BY a.id
                ) subquery
                WHERE announcements.id = subquery.id;
            ");
        }
    }
}
