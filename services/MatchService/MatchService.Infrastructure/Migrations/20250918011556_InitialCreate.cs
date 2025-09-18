using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MatchService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "match_service");

            migrationBuilder.CreateTable(
                name: "matches",
                schema: "match_service",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user1_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user2_id = table.Column<Guid>(type: "uuid", nullable: false),
                    matched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    unmatched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    unmatched_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.CheckConstraint("CK_Match_DifferentUsers", "user1_id != user2_id");
                    table.CheckConstraint("CK_Match_UnmatchLogic", "(is_active = true AND unmatched_at IS NULL AND unmatched_by_user_id IS NULL) OR (is_active = false AND unmatched_at IS NOT NULL AND unmatched_by_user_id IS NOT NULL)");
                });

            migrationBuilder.CreateTable(
                name: "swipes",
                schema: "match_service",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    swiper_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direction = table.Column<string>(type: "text", nullable: false),
                    swiped_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swipes", x => x.id);
                    table.CheckConstraint("CK_Swipe_DifferentUsers", "swiper_id != target_user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_IsActive",
                schema: "match_service",
                table: "matches",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Users",
                schema: "match_service",
                table: "matches",
                columns: new[] { "user1_id", "user2_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_Direction",
                schema: "match_service",
                table: "swipes",
                column: "direction");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwiperId",
                schema: "match_service",
                table: "swipes",
                column: "swiper_id");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_TargetUserId",
                schema: "match_service",
                table: "swipes",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_Users",
                schema: "match_service",
                table: "swipes",
                columns: new[] { "swiper_id", "target_user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches",
                schema: "match_service");

            migrationBuilder.DropTable(
                name: "swipes",
                schema: "match_service");
        }
    }
}
