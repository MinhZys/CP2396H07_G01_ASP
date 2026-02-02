using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassId",
                table: "Guests",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamRoom",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guests_ClassId",
                table: "Guests",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_ClassId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "ExamRoom",
                table: "Guests");
        }
    }
}
