using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NextUp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Team_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coach",
                columns: table => new
                {
                    CoachId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: true),
                    ExperienceYears = table.Column<int>(type: "integer", nullable: false),
                    Specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Certification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coach", x => x.CoachId);
                    table.ForeignKey(
                        name: "FK_Coach_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "TeamId");
                    table.ForeignKey(
                        name: "FK_Coach_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: false),
                    GameDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: false),
                    AwayScore = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Season = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Game_Team_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Team",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Team_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Team",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    JerseyNumber = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_Player_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Player_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameNote",
                columns: table => new
                {
                    GameNoteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    NoteType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameNote", x => x.GameNoteId);
                    table.ForeignKey(
                        name: "FK_GameNote_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameNote_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameStats",
                columns: table => new
                {
                    PlayerGameStatsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    PassingYards = table.Column<int>(type: "integer", nullable: false),
                    PassingTouchdowns = table.Column<int>(type: "integer", nullable: false),
                    Interceptions = table.Column<int>(type: "integer", nullable: false),
                    RushingYards = table.Column<int>(type: "integer", nullable: false),
                    RushingTouchdowns = table.Column<int>(type: "integer", nullable: false),
                    ReceivingYards = table.Column<int>(type: "integer", nullable: false),
                    ReceivingTouchdowns = table.Column<int>(type: "integer", nullable: false),
                    Receptions = table.Column<int>(type: "integer", nullable: false),
                    Tackles = table.Column<int>(type: "integer", nullable: false),
                    Assists = table.Column<int>(type: "integer", nullable: false),
                    Sacks = table.Column<int>(type: "integer", nullable: false),
                    ForcedFumbles = table.Column<int>(type: "integer", nullable: false),
                    InterceptionsDef = table.Column<int>(type: "integer", nullable: false),
                    FieldGoalsMade = table.Column<int>(type: "integer", nullable: false),
                    FieldGoalsAttempted = table.Column<int>(type: "integer", nullable: false),
                    ExtraPointsMade = table.Column<int>(type: "integer", nullable: false),
                    ExtraPointsAttempted = table.Column<int>(type: "integer", nullable: false),
                    MinutesPlayed = table.Column<int>(type: "integer", nullable: false),
                    RecordedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameStats", x => x.PlayerGameStatsId);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Users_RecordedBy",
                        column: x => x.RecordedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGoal",
                columns: table => new
                {
                    PlayerGoalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    GoalType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    CurrentValue = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGoal", x => x.PlayerGoalId);
                    table.ForeignKey(
                        name: "FK_PlayerGoal_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerNote",
                columns: table => new
                {
                    PlayerNoteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNote", x => x.PlayerNoteId);
                    table.ForeignKey(
                        name: "FK_PlayerNote_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerNote_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_coach_team",
                table: "Coach",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "idx_coach_user",
                table: "Coach",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_away_team",
                table: "Game",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "idx_game_date",
                table: "Game",
                column: "GameDate");

            migrationBuilder.CreateIndex(
                name: "idx_home_team",
                table: "Game",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "idx_gamenote_coach",
                table: "GameNote",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "idx_gamenote_game",
                table: "GameNote",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "idx_player_team",
                table: "Player",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "idx_player_user",
                table: "Player",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_player_game_unique",
                table: "PlayerGameStats",
                columns: new[] { "PlayerId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_stats_game",
                table: "PlayerGameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "idx_stats_player",
                table: "PlayerGameStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameStats_RecordedBy",
                table: "PlayerGameStats",
                column: "RecordedBy");

            migrationBuilder.CreateIndex(
                name: "idx_goal_player",
                table: "PlayerGoal",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "idx_playernote_coach",
                table: "PlayerNote",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "idx_playernote_player",
                table: "PlayerNote",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "idx_team_coach",
                table: "Team",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coach");

            migrationBuilder.DropTable(
                name: "GameNote");

            migrationBuilder.DropTable(
                name: "PlayerGameStats");

            migrationBuilder.DropTable(
                name: "PlayerGoal");

            migrationBuilder.DropTable(
                name: "PlayerNote");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
