using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class addapikey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

                  }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "AspNetUsers");

        }
    }
}
