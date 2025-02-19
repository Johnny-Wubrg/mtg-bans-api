using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class Slugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "formats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE formats SET slug = lower(name)");

            migrationBuilder.CreateIndex(
                name: "ix_formats_name",
                table: "formats",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_formats_slug",
                table: "formats",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_expansions_code",
                table: "expansions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_expansions_name",
                table: "expansions",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_formats_name",
                table: "formats");

            migrationBuilder.DropIndex(
                name: "ix_formats_slug",
                table: "formats");

            migrationBuilder.DropIndex(
                name: "ix_expansions_code",
                table: "expansions");

            migrationBuilder.DropIndex(
                name: "ix_expansions_name",
                table: "expansions");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "formats");
        }
    }
}
