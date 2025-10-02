using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePhoneNumberNullableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthUsers_PhoneNumber",
                table: "AuthUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AuthUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_PhoneNumber",
                table: "AuthUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "\"PhoneNumber\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthUsers_PhoneNumber",
                table: "AuthUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AuthUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_PhoneNumber",
                table: "AuthUsers",
                column: "PhoneNumber",
                unique: true);
        }
    }
}
