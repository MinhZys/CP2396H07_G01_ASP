using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordFromGuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Guests");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 9, 57, 370, DateTimeKind.Local).AddTicks(4567));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 9, 57, 370, DateTimeKind.Local).AddTicks(4582));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 9, 57, 370, DateTimeKind.Local).AddTicks(4583));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Guests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 1, 57, 56, 813, DateTimeKind.Local).AddTicks(254));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 1, 57, 56, 813, DateTimeKind.Local).AddTicks(279));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 1, 57, 56, 813, DateTimeKind.Local).AddTicks(280));
        }
    }
}
