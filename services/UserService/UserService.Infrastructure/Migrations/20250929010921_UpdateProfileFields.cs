using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampusLife",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommunicationPreference",
                table: "Profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealBreakers",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuturePlan",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterestedIn",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LookingFor",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoveLanguage",
                table: "Profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudyStyle",
                table: "Profiles",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "Profiles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Voice",
                table: "Profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeekendHobby",
                table: "Profiles",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zodiac",
                table: "Profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampusLife",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CommunicationPreference",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "DealBreakers",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "FuturePlan",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "InterestedIn",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "LookingFor",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "LoveLanguage",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "StudyStyle",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "University",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Voice",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "WeekendHobby",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Zodiac",
                table: "Profiles");
        }
    }
}
