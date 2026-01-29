using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VNPayTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    VnpTxnRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VnpAmount = table.Column<long>(type: "bigint", nullable: false),
                    VnpOrderInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VnpOrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VnpCreateDate = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    VnpResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    VnpTransactionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VnpBankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VnpBankTranNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VnpCardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VnpPayDate = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VNPayTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VNPayTransactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 19, 0, 574, DateTimeKind.Local).AddTicks(10));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 19, 0, 574, DateTimeKind.Local).AddTicks(36));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 19, 0, 574, DateTimeKind.Local).AddTicks(37));

            migrationBuilder.CreateIndex(
                name: "IX_VNPayTransactions_PaymentId",
                table: "VNPayTransactions",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VNPayTransactions");

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 9, 39, 42, 127, DateTimeKind.Local).AddTicks(9049));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 9, 39, 42, 127, DateTimeKind.Local).AddTicks(9077));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 9, 39, 42, 127, DateTimeKind.Local).AddTicks(9081));
        }
    }
}
