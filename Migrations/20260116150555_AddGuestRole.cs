using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { "4", "Prospective Student", "Guest" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "4");
        }
    }
}
