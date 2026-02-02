using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class FixClassStatusData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Classes SET Status = 'Active' WHERE Status = N'Thi thử'");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 46, 17, 900, DateTimeKind.Local).AddTicks(1555));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 46, 17, 900, DateTimeKind.Local).AddTicks(1570));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 46, 17, 900, DateTimeKind.Local).AddTicks(1571));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5455));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5470));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5471));
        }
    }
}
