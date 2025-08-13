using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class MultipleImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uris_normal",
                table: "printings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uris_png",
                table: "printings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uris_small",
                table: "printings",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE printings
                SET scryfall_image_uris_png = scryfall_image_uri,
                    scryfall_image_uris_small = REPLACE(REPLACE(scryfall_image_uri, '/png/', '/small/'), '.png', '.jpg'),
                    scryfall_image_uris_normal = REPLACE(REPLACE(scryfall_image_uri, '/png/', '/normal/'), '.png', '.jpg')
            ");
            
            migrationBuilder.DropColumn(
                name: "scryfall_image_uri",
                table: "printings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "scryfall_image_uri",
                table: "printings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE printings
                SET scryfall_image_uri = scryfall_image_uris_png
            ");
            
            migrationBuilder.DropColumn(
                name: "scryfall_image_uris_normal",
                table: "printings");

            migrationBuilder.DropColumn(
                name: "scryfall_image_uris_png",
                table: "printings");

            migrationBuilder.DropColumn(
                name: "scryfall_image_uris_small",
                table: "printings");
        }
    }
}
