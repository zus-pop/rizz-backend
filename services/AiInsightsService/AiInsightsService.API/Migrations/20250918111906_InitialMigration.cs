using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AiInsightsService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiInsights",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SummaryText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CompatibilityScore = table.Column<float>(type: "real", precision: 5, scale: 2, nullable: false),
                    PersonalityTags = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiInsights", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiInsights_CompatibilityScore",
                table: "AiInsights",
                column: "CompatibilityScore");

            migrationBuilder.CreateIndex(
                name: "IX_AiInsights_UpdatedAt",
                table: "AiInsights",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiInsights");
        }
    }
}