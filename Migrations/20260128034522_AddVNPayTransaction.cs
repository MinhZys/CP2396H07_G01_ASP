using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP2396H07_G01.Migrations
{
    /// <inheritdoc />
    public partial class AddVNPayTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VNPayTransactions");

            migrationBuilder.DropColumn(
                name: "VnpBankTranNo",
                table: "VNPayTransactions");

            migrationBuilder.DropColumn(
                name: "VnpCardType",
                table: "VNPayTransactions");

            migrationBuilder.DropColumn(
                name: "VnpOrderType",
                table: "VNPayTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "VnpTxnRef",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "VnpTransactionNo",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "VnpResponseCode",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "VnpPayDate",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.AlterColumn<string>(
                name: "VnpOrderInfo",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "VnpCreateDate",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.AlterColumn<string>(
                name: "VnpBankCode",
                table: "VNPayTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 45, 20, 750, DateTimeKind.Local).AddTicks(9178));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 45, 20, 750, DateTimeKind.Local).AddTicks(9203));

            migrationBuilder.UpdateData(
                table: "ClassCategories",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 28, 10, 45, 20, 750, DateTimeKind.Local).AddTicks(9205));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VnpTxnRef",
                table: "VNPayTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VnpTransactionNo",
                table: "VNPayTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VnpResponseCode",
                table: "VNPayTransactions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VnpPayDate",
                table: "VNPayTransactions",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VnpOrderInfo",
                table: "VNPayTransactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VnpCreateDate",
                table: "VNPayTransactions",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VnpBankCode",
                table: "VNPayTransactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VNPayTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VnpBankTranNo",
                table: "VNPayTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VnpCardType",
                table: "VNPayTransactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VnpOrderType",
                table: "VNPayTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
        }
    }
}
