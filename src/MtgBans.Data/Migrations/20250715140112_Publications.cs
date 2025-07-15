using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class Publications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the publications table first
            migrationBuilder.CreateTable(
                name: "publications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    date_published = table.Column<DateOnly>(type: "date", nullable: false),
                    uri = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publications", x => x.id);
                });

            // Create the junction table
            migrationBuilder.CreateTable(
                name: "announcement_publication",
                columns: table => new
                {
                    announcements_id = table.Column<int>(type: "integer", nullable: false),
                    sources_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcement_publication", x => new { x.announcements_id, x.sources_id });
                    table.ForeignKey(
                        name: "fk_announcement_publication_announcements_announcements_id",
                        column: x => x.announcements_id,
                        principalTable: "announcements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_announcement_publication_publications_sources_id",
                        column: x => x.sources_id,
                        principalTable: "publications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_announcement_publication_sources_id",
                table: "announcement_publication",
                column: "sources_id");

            // Migrate existing data from sources array to publications table
            migrationBuilder.Sql(@"
                -- Insert distinct URIs into publications table with title and date from earliest announcement
                INSERT INTO publications (uri, title, date_published)
                SELECT DISTINCT 
                    source_uri,
                    (SELECT summary 
                     FROM announcements a2 
                     WHERE a2.sources @> ARRAY[source_uri]::text[]
                     ORDER BY a2.date_announced ASC 
                     LIMIT 1) as title,
                    (SELECT date_announced 
                     FROM announcements a2 
                     WHERE a2.sources @> ARRAY[source_uri]::text[]
                     ORDER BY a2.date_announced ASC 
                     LIMIT 1) as date_published
                FROM (
                    SELECT unnest(sources) as source_uri
                    FROM announcements
                    WHERE sources IS NOT NULL
                ) distinct_sources
                WHERE source_uri IS NOT NULL AND source_uri != '';
            ");

            // Create relationships in junction table
            migrationBuilder.Sql(@"
                -- Insert relationships into announcement_publication junction table
                INSERT INTO announcement_publication (announcements_id, sources_id)
                SELECT DISTINCT 
                    a.id as announcements_id,
                    p.id as sources_id
                FROM announcements a
                CROSS JOIN unnest(a.sources) as source_uri
                JOIN publications p ON p.uri = source_uri
                WHERE a.sources IS NOT NULL;
            ");

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

            // Drop the new tables
            migrationBuilder.DropTable(
                name: "announcement_publication");

            migrationBuilder.DropTable(
                name: "publications");
        }
    }
}