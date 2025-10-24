using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPlayerGameStatsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockedKicks",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CleanSnaps",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CompletionPercentage",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "FieldGoalAttempts",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldGoalMade",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Fumbles",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InterceptionReturnTouchDown",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InterceptionReturnYards",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongestFieldGoal",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongestPass",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongestReception",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongestRushing",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PancakeBlocks",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassBreakups",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassingAttempts",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Penalties",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pressures",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivingTDs",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RushingAttempts",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RushingTDs",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sacked",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SacksAllowed",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SnapAccuracy",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SnapsPlayed",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TacklesForLoss",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Targets",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSnaps",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalTackles",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Touchbacks",
                table: "PlayerGameStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "YardsPerPassAttempt",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YardsPerPunt",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YardsPerReception",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YardsPerRushAttempt",
                table: "PlayerGameStats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedKicks",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "CleanSnaps",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "CompletionPercentage",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "FieldGoalAttempts",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "FieldGoalMade",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Fumbles",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "InterceptionReturnTouchDown",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "InterceptionReturnYards",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "LongestFieldGoal",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "LongestPass",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "LongestReception",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "LongestRushing",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "PancakeBlocks",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "PassBreakups",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "PassingAttempts",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Penalties",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Pressures",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "ReceivingTDs",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "RushingAttempts",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "RushingTDs",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Sacked",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "SacksAllowed",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "SnapAccuracy",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "SnapsPlayed",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "TacklesForLoss",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Targets",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "TotalSnaps",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "TotalTackles",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "Touchbacks",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "YardsPerPassAttempt",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "YardsPerPunt",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "YardsPerReception",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "YardsPerRushAttempt",
                table: "PlayerGameStats");
        }
    }
}
