using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpCodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OtpCodes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AuthUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AuthUsers");
        }
    }
}
