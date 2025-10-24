using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "Game",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week",
                table: "Game");
        }
    }
}
