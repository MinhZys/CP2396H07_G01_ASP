using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentExamSessionForClassExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EntranceExamId",
                table: "StudentExamSessions",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)");

            migrationBuilder.AddColumn<string>(
                name: "ClassExamId",
                table: "StudentExamSessions",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 37, 54, 693, DateTimeKind.Local).AddTicks(7786));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 37, 54, 693, DateTimeKind.Local).AddTicks(7807));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 37, 54, 693, DateTimeKind.Local).AddTicks(7809));

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamSessions_ClassExamId",
                table: "StudentExamSessions",
                column: "ClassExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamSessions_ClassExams_ClassExamId",
                table: "StudentExamSessions",
                column: "ClassExamId",
                principalTable: "ClassExams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamSessions_ClassExams_ClassExamId",
                table: "StudentExamSessions");

            migrationBuilder.DropIndex(
                name: "IX_StudentExamSessions_ClassExamId",
                table: "StudentExamSessions");

            migrationBuilder.DropColumn(
                name: "ClassExamId",
                table: "StudentExamSessions");

            migrationBuilder.AlterColumn<string>(
                name: "EntranceExamId",
                table: "StudentExamSessions",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 34, 35, 568, DateTimeKind.Local).AddTicks(2183));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 34, 35, 568, DateTimeKind.Local).AddTicks(2196));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 34, 35, 568, DateTimeKind.Local).AddTicks(2197));
        }
    }
}
