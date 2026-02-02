using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelReasonAndClassLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassLessons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ClassId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    LessonId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLessons_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 19, 41, 19, 123, DateTimeKind.Local).AddTicks(1187));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 19, 41, 19, 123, DateTimeKind.Local).AddTicks(1199));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 19, 41, 19, 123, DateTimeKind.Local).AddTicks(1200));

            migrationBuilder.CreateIndex(
                name: "IX_ClassLessons_ClassId_LessonId",
                table: "ClassLessons",
                columns: new[] { "ClassId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLessons_LessonId",
                table: "ClassLessons",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassLessons");

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
        }
    }
}
