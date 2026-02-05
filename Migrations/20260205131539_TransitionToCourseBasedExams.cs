using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class TransitionToCourseBasedExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassExams_Subjects_SubjectId",
                table: "ClassExams");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamPapers_Subjects_SubjectId",
                table: "ExamPapers");

            migrationBuilder.DropIndex(
                name: "IX_ExamPapers_SubjectId",
                table: "ExamPapers");

            migrationBuilder.DropIndex(
                name: "IX_ClassExams_SubjectId",
                table: "ClassExams");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ExamPapers");

            migrationBuilder.DropColumn(
                name: "IsScorePublished",
                table: "EntranceExams");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ClassExams");

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "ExamPapers",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "ClassExams",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 20, 15, 38, 924, DateTimeKind.Local).AddTicks(7967));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 20, 15, 38, 924, DateTimeKind.Local).AddTicks(7984));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 20, 15, 38, 924, DateTimeKind.Local).AddTicks(7986));

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_CourseId",
                table: "ExamPapers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExams_CourseId",
                table: "ClassExams",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExams_Courses_CourseId",
                table: "ClassExams",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamPapers_Courses_CourseId",
                table: "ExamPapers",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassExams_Courses_CourseId",
                table: "ClassExams");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamPapers_Courses_CourseId",
                table: "ExamPapers");

            migrationBuilder.DropIndex(
                name: "IX_ExamPapers_CourseId",
                table: "ExamPapers");

            migrationBuilder.DropIndex(
                name: "IX_ClassExams_CourseId",
                table: "ClassExams");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ExamPapers");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ClassExams");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "ExamPapers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsScorePublished",
                table: "EntranceExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "ClassExams",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 19, 24, 51, 910, DateTimeKind.Local).AddTicks(8582));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 19, 24, 51, 910, DateTimeKind.Local).AddTicks(8600));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 5, 19, 24, 51, 910, DateTimeKind.Local).AddTicks(8601));

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_SubjectId",
                table: "ExamPapers",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExams_SubjectId",
                table: "ClassExams",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExams_Subjects_SubjectId",
                table: "ClassExams",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamPapers_Subjects_SubjectId",
                table: "ExamPapers",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
