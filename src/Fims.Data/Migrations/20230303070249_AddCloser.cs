using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fims.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCloser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloserName",
                table: "TSheets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingEndDateTime",
                table: "TSheets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingStartDateTime",
                table: "TSheets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsClosingCompleted",
                table: "TSheets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloserName",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "ClosingEndDateTime",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "ClosingStartDateTime",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "IsClosingCompleted",
                table: "TSheets");
        }
    }
}
