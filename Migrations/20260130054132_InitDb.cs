using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CenterId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Classes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RoomLocation",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 12, 41, 31, 74, DateTimeKind.Local).AddTicks(7158));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 12, 41, 31, 74, DateTimeKind.Local).AddTicks(7206));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 12, 41, 31, 74, DateTimeKind.Local).AddTicks(7209));

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CenterId",
                table: "Classes",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Centers_CenterId",
                table: "Classes",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Centers_CenterId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_CenterId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RoomLocation",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Classes");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 11, 13, 43, 457, DateTimeKind.Local).AddTicks(4293));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 11, 13, 43, 457, DateTimeKind.Local).AddTicks(4311));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 11, 13, 43, 457, DateTimeKind.Local).AddTicks(4312));
        }
    }
}
