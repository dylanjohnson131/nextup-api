using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Team",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conference",
                table: "Team",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Team",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mascot",
                table: "Team",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "School",
                table: "Team",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Team",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Conference",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Mascot",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "School",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Team");
        }
    }
}
