using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardSort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sort_name",
                table: "cards",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("UPDATE cards SET sort_name = regexp_replace(lower(name), '[^a-z]+', '', 'g')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sort_name",
                table: "cards");
        }
    }
}
