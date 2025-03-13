using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgBans.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardLegalityStatusCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_event_card_legality_status_status_id",
                table: "card_legality_event");

            migrationBuilder.DropPrimaryKey(
                name: "pk_card_legality_status",
                table: "card_legality_status");

            migrationBuilder.DropColumn(
                name: "type",
                table: "card_legality_event");

            migrationBuilder.RenameTable(
                name: "card_legality_status",
                newName: "card_legality_statuses");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "card_legality_event",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_card_legality_statuses",
                table: "card_legality_statuses",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_event_card_legality_statuses_status_id",
                table: "card_legality_event",
                column: "status_id",
                principalTable: "card_legality_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_legality_event_card_legality_statuses_status_id",
                table: "card_legality_event");

            migrationBuilder.DropPrimaryKey(
                name: "pk_card_legality_statuses",
                table: "card_legality_statuses");

            migrationBuilder.RenameTable(
                name: "card_legality_statuses",
                newName: "card_legality_status");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "card_legality_event",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "card_legality_event",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql(@"
                UPDATE card_legality_event 
                SET type = case when status_id >= 7  
                    then 2
                    else status_id - 1
                end
            ");

            migrationBuilder.AddPrimaryKey(
                name: "pk_card_legality_status",
                table: "card_legality_status",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_legality_event_card_legality_status_status_id",
                table: "card_legality_event",
                column: "status_id",
                principalTable: "card_legality_status",
                principalColumn: "id");
        }
    }
}
