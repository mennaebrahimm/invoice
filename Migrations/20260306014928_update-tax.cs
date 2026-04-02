using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatetax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommercialRegisterNumber",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FreelanceDocumentNumber",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5114));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5183));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6061));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5957));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5856));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5915));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5937));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ed",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6086));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6074));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6104));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(5975));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6158));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6123));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6014));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "tp",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 4, 49, 26, 488, DateTimeKind.Unspecified).AddTicks(6027));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialRegisterNumber",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "FreelanceDocumentNumber",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Taxes");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(2733));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(2792));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3560));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3486));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3317));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3368));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3384));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3684));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ed",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3592));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3576));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3608));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3511));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3651));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3629));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3528));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "tp",
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 4, 53, 20, 336, DateTimeKind.Unspecified).AddTicks(3544));
        }
    }
}
