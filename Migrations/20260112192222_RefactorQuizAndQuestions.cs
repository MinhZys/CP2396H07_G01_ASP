using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class RefactorQuizAndQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "TotalQuestions",
                table: "Quizzes",
                newName: "PassScore");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Quizzes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "QuizQuestions",
                newName: "OptionD");

            migrationBuilder.AddColumn<string>(
                name: "LessonId",
                table: "Quizzes",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectOption",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionA",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionB",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionC",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonId",
                table: "Quizzes",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Lessons_LessonId",
                table: "Quizzes",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Lessons_LessonId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_LessonId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CorrectOption",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "OptionA",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "OptionB",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "OptionC",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "QuizQuestions");

            migrationBuilder.RenameColumn(
                name: "PassScore",
                table: "Quizzes",
                newName: "TotalQuestions");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Quizzes",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "OptionD",
                table: "QuizQuestions",
                newName: "Answer");

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Quizzes",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MaxScore",
                table: "Quizzes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
