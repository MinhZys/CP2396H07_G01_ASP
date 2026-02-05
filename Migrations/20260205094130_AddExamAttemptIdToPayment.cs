using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAttemptIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamAttemptId",
                table: "Payments",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 41, 30, 212, DateTimeKind.Local).AddTicks(7460));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 41, 30, 212, DateTimeKind.Local).AddTicks(7482));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 16, 41, 30, 212, DateTimeKind.Local).AddTicks(7484));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ExamAttemptId",
                table: "Payments",
                column: "ExamAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ExamAttempts_ExamAttemptId",
                table: "Payments",
                column: "ExamAttemptId",
                principalTable: "ExamAttempts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ExamAttempts_ExamAttemptId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ExamAttemptId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExamAttemptId",
                table: "Payments");

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
        }
    }
}
