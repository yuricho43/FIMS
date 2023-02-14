using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fims.Data.Migrations
{
    /// <inheritdoc />
    public partial class NoUserIdInTSheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TItems_TSheets_TSheetId",
                table: "TItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TSheets_AspNetUsers_UserId",
                table: "TSheets");

            migrationBuilder.DropIndex(
                name: "IX_TSheets_IsDeleted",
                table: "TSheets");

            migrationBuilder.DropIndex(
                name: "IX_TSheets_UserId",
                table: "TSheets");

            migrationBuilder.DropIndex(
                name: "IX_TItems_IsDeleted",
                table: "TItems");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TSheets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "TItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TItems");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HangulName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EnglishName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TItems_TSheets_TSheetId",
                table: "TItems",
                column: "TSheetId",
                principalTable: "TSheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TItems_TSheets_TSheetId",
                table: "TItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "TSheets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TSheets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TSheets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "TItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "HangulName",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EnglishName",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TSheets_IsDeleted",
                table: "TSheets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TSheets_UserId",
                table: "TSheets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TItems_IsDeleted",
                table: "TItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_TItems_TSheets_TSheetId",
                table: "TItems",
                column: "TSheetId",
                principalTable: "TSheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TSheets_AspNetUsers_UserId",
                table: "TSheets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
