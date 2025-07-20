using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class PublicationArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "publications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "publication_archive",
                columns: table => new
                {
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    content_markdown = table.Column<string>(type: "text", nullable: true),
                    content_html = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_archive", x => x.publication_id);
                    table.ForeignKey(
                        name: "fk_publication_archive_publications_publication_id",
                        column: x => x.publication_id,
                        principalTable: "publications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "publication_archive");

            migrationBuilder.DropColumn(
                name: "active",
                table: "publications");
        }
    }
}
