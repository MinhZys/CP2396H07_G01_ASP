using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class RefactorClassAndAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Classrooms_ClassroomId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Courses_CourseId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Users_InstructorId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassroomId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_CourseId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_InstructorId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassroomId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "OfflineFee",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RequiredRoomType",
                table: "ClassCategories");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Classes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Classes",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSeats",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ClassCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ClassCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermOrExamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    AssignmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5455), "Standard classrooms" });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5470), "Computer labs" });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 17, 2, 42, 34, 586, DateTimeKind.Local).AddTicks(5471), "Virtual classes" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassId",
                table: "Assignments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_InstructorId",
                table: "Assignments",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "NumberOfSeats",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClassCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ClassCategories");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Classes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Classes",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "ClassroomId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Classes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OfflineFee",
                table: "Classes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredRoomType",
                table: "ClassCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "RequiredRoomType",
                value: "TheoryRoom");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "RequiredRoomType",
                value: "LabRoom");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "RequiredRoomType",
                value: "Online");

            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Capacity", "IsActive", "Name", "RoomType" },
                values: new object[,]
                {
                    { "1", 30, true, "Room 101", "TheoryRoom" },
                    { "2", 20, true, "Lab A", "LabRoom" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassroomId",
                table: "Classes",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CourseId",
                table: "Classes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_InstructorId",
                table: "Classes",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Classrooms_ClassroomId",
                table: "Classes",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Courses_CourseId",
                table: "Classes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Users_InstructorId",
                table: "Classes",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
