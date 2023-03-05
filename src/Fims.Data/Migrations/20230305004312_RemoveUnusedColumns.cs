using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fims.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosingCompleted",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "IsInspectionCompleted",
                table: "TSheets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosingCompleted",
                table: "TSheets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInspectionCompleted",
                table: "TSheets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
