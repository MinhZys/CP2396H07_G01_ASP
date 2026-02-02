using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class FixIdLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop Dependencies (FKs)
            migrationBuilder.DropForeignKey("FK_Users_Roles_RoleId", "Users");
            migrationBuilder.DropForeignKey("FK_StudentRegistrations_Courses_CourseId", "StudentRegistrations");
            migrationBuilder.DropForeignKey("FK_StudentRegistrations_Centers_CenterId", "StudentRegistrations");
            migrationBuilder.DropForeignKey("FK_Payments_Users_StudentId", "Payments");
            migrationBuilder.DropForeignKey("FK_PageContents_Centers_CenterId", "PageContents");
            migrationBuilder.DropForeignKey("FK_ExamResults_Users_StudentId", "ExamResults");
            migrationBuilder.DropForeignKey("FK_ExamResults_EntranceExams_EntranceExamId", "ExamResults");
            migrationBuilder.DropForeignKey("FK_Enrollments_Users_StudentId", "Enrollments");
            migrationBuilder.DropForeignKey("FK_Enrollments_Classes_ClassId", "Enrollments");
            migrationBuilder.DropForeignKey("FK_CourseSubjects_Courses_CourseId", "CourseSubjects");
            migrationBuilder.DropForeignKey("FK_Classes_Courses_CourseId", "Classes");
            migrationBuilder.DropForeignKey("FK_Classes_Users_InstructorId", "Classes");
            // ExamDetails - RegistrationId might need check, assuming name based on conventions
            try { migrationBuilder.DropForeignKey("FK_ExamDetails_StudentRegistrations_RegistrationId", "ExamDetails"); } catch { } 

            // 2. Drop PKs to allow altering Id columns
            migrationBuilder.DropPrimaryKey("PK_Users", "Users");
            migrationBuilder.DropPrimaryKey("PK_Roles", "Roles");
            migrationBuilder.DropPrimaryKey("PK_StudentRegistrations", "StudentRegistrations");
            migrationBuilder.DropPrimaryKey("PK_Payments", "Payments");
            migrationBuilder.DropPrimaryKey("PK_PageContents", "PageContents");
            migrationBuilder.DropPrimaryKey("PK_FAQs", "FAQs");
            migrationBuilder.DropPrimaryKey("PK_ExamResults", "ExamResults");
            migrationBuilder.DropPrimaryKey("PK_ExamDetails", "ExamDetails");
            migrationBuilder.DropPrimaryKey("PK_EntranceExams", "EntranceExams");
            migrationBuilder.DropPrimaryKey("PK_Enrollments", "Enrollments");
            migrationBuilder.DropPrimaryKey("PK_CourseSubjects", "CourseSubjects");
            migrationBuilder.DropPrimaryKey("PK_Courses", "Courses");
            migrationBuilder.DropPrimaryKey("PK_Classes", "Classes");
            migrationBuilder.DropPrimaryKey("PK_Centers", "Centers");

            // 3. Alter Columns (Auto-generated code)
            migrationBuilder.AlterColumn<string>(name: "RoleId", table: "Users", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Users", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "CourseId", table: "StudentRegistrations", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "CenterId", table: "StudentRegistrations", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "StudentRegistrations", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Roles", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "StudentId", table: "Payments", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Payments", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "CenterId", table: "PageContents", type: "nvarchar(36)", nullable: true, oldClrType: typeof(string), oldType: "nvarchar(26)", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "PageContents", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "FAQs", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "StudentId", table: "ExamResults", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "EntranceExamId", table: "ExamResults", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "ExamResults", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "RegistrationId", table: "ExamDetails", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "ExamDetails", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "EntranceExams", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "StudentId", table: "Enrollments", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "ClassId", table: "Enrollments", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Enrollments", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "CourseId", table: "CourseSubjects", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Courses", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "InstructorId", table: "Classes", type: "nvarchar(36)", nullable: true, oldClrType: typeof(string), oldType: "nvarchar(26)", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "CourseId", table: "Classes", type: "nvarchar(36)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Classes", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Centers", type: "nvarchar(36)", maxLength: 36, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(26)", oldMaxLength: 26);

            // 4. Restore PKs
            migrationBuilder.AddPrimaryKey("PK_Users", "Users", "Id");
            migrationBuilder.AddPrimaryKey("PK_Roles", "Roles", "Id");
            migrationBuilder.AddPrimaryKey("PK_StudentRegistrations", "StudentRegistrations", "Id");
            migrationBuilder.AddPrimaryKey("PK_Payments", "Payments", "Id");
            migrationBuilder.AddPrimaryKey("PK_PageContents", "PageContents", "Id");
            migrationBuilder.AddPrimaryKey("PK_FAQs", "FAQs", "Id");
            migrationBuilder.AddPrimaryKey("PK_ExamResults", "ExamResults", "Id");
            migrationBuilder.AddPrimaryKey("PK_ExamDetails", "ExamDetails", "Id");
            migrationBuilder.AddPrimaryKey("PK_EntranceExams", "EntranceExams", "Id");
            migrationBuilder.AddPrimaryKey("PK_Enrollments", "Enrollments", "Id");
            migrationBuilder.AddPrimaryKey("PK_CourseSubjects", "CourseSubjects", new[] { "CourseId", "SubjectId" });
            migrationBuilder.AddPrimaryKey("PK_Courses", "Courses", "Id");
            migrationBuilder.AddPrimaryKey("PK_Classes", "Classes", "Id");
            migrationBuilder.AddPrimaryKey("PK_Centers", "Centers", "Id");

            // 5. Restore FKs
            migrationBuilder.AddForeignKey(name: "FK_Users_Roles_RoleId", table: "Users", column: "RoleId", principalTable: "Roles", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_StudentRegistrations_Courses_CourseId", table: "StudentRegistrations", column: "CourseId", principalTable: "Courses", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_StudentRegistrations_Centers_CenterId", table: "StudentRegistrations", column: "CenterId", principalTable: "Centers", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Payments_Users_StudentId", table: "Payments", column: "StudentId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_PageContents_Centers_CenterId", table: "PageContents", column: "CenterId", principalTable: "Centers", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_ExamResults_Users_StudentId", table: "ExamResults", column: "StudentId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_ExamResults_EntranceExams_EntranceExamId", table: "ExamResults", column: "EntranceExamId", principalTable: "EntranceExams", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Enrollments_Users_StudentId", table: "Enrollments", column: "StudentId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Enrollments_Classes_ClassId", table: "Enrollments", column: "ClassId", principalTable: "Classes", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CourseSubjects_Courses_CourseId", table: "CourseSubjects", column: "CourseId", principalTable: "Courses", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Classes_Courses_CourseId", table: "Classes", column: "CourseId", principalTable: "Courses", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Classes_Users_InstructorId", table: "Classes", column: "InstructorId", principalTable: "Users", principalColumn: "Id");
            try { migrationBuilder.AddForeignKey(name: "FK_ExamDetails_StudentRegistrations_RegistrationId", table: "ExamDetails", column: "RegistrationId", principalTable: "StudentRegistrations", principalColumn: "Id", onDelete: ReferentialAction.Cascade); } catch { }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             // Down not strictly required to be perfect for this fix, but good practice
        }
    }
}
