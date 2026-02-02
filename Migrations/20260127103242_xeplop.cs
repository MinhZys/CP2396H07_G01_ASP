using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class xeplop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassAssignments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ClassId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassAssignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassAssignments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 27, 17, 32, 42, 303, DateTimeKind.Local).AddTicks(2037));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 27, 17, 32, 42, 303, DateTimeKind.Local).AddTicks(2050));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 27, 17, 32, 42, 303, DateTimeKind.Local).AddTicks(2052));

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_ClassId",
                table: "ClassAssignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_StudentId",
                table: "ClassAssignments",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassAssignments");

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
    }
}
