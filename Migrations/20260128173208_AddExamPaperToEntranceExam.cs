using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class AddExamPaperToEntranceExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamPaperId",
                table: "EntranceExams",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 0, 32, 7, 542, DateTimeKind.Local).AddTicks(9413));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 0, 32, 7, 542, DateTimeKind.Local).AddTicks(9427));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 0, 32, 7, 542, DateTimeKind.Local).AddTicks(9428));

            migrationBuilder.CreateIndex(
                name: "IX_EntranceExams_ExamPaperId",
                table: "EntranceExams",
                column: "ExamPaperId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceExams_ExamPapers_ExamPaperId",
                table: "EntranceExams",
                column: "ExamPaperId",
                principalTable: "ExamPapers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntranceExams_ExamPapers_ExamPaperId",
                table: "EntranceExams");

            migrationBuilder.DropIndex(
                name: "IX_EntranceExams_ExamPaperId",
                table: "EntranceExams");

            migrationBuilder.DropColumn(
                name: "ExamPaperId",
                table: "EntranceExams");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 21, 14, 29, 200, DateTimeKind.Local).AddTicks(7563));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 21, 14, 29, 200, DateTimeKind.Local).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 21, 14, 29, 200, DateTimeKind.Local).AddTicks(7608));
        }
    }
}
