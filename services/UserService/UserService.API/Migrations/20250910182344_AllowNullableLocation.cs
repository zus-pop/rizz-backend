using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace UserService.API.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullableLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Users",
                type: "geometry (point)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry (point)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Users",
                type: "geometry (point)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point)",
                oldNullable: true);
        }
    }
}
