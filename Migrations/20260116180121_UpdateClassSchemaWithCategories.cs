using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassSchemaWithCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.AddColumn<string>(
                name: "ClassCategoryId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassroomId",
                table: "Classes",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredRoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ClassCategories",
                columns: new[] { "Id", "IsActive", "Name", "RequiredRoomType" },
                values: new object[,]
                {
                    { "1", true, "Theory", "TheoryRoom" },
                    { "2", true, "Lab", "LabRoom" },
                    { "3", true, "Online", "Online" }
                });

            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Capacity", "IsActive", "Name", "RoomType" },
                values: new object[,]
                {
                    { "1", 30, true, "Room 101", "TheoryRoom" },
                    { "2", 20, true, "Lab A", "LabRoom" }
                });

            // FIX: Update existing Classes to have valid ClassCategoryId
            migrationBuilder.Sql("UPDATE Classes SET ClassCategoryId = '1' WHERE ClassCategoryId = '' OR ClassCategoryId IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassCategoryId",
                table: "Classes",
                column: "ClassCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassroomId",
                table: "Classes",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_ClassCategories_ClassCategoryId",
                table: "Classes",
                column: "ClassCategoryId",
                principalTable: "ClassCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Classrooms_ClassroomId",
                table: "Classes",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_ClassCategories_ClassCategoryId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Classrooms_ClassroomId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "ClassCategories");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassCategoryId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassroomId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassCategoryId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassroomId",
                table: "Classes");

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[] { "1", "Awarded upon completing all course requirements.", true, "Certificate of Completion" });
        }
    }
}
