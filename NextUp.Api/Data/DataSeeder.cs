using Microsoft.EntityFrameworkCore;
using NextUp.Models;
using NextUp.Api.Services;

namespace NextUp.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(NextUpDbContext context, IPasswordService passwordService)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Create Coach Users
            var coachUser1 = new User
            {
                FirstName = "Mike",
                LastName = "Johnson",
                Email = "coach.johnson@nextup.com",
                PasswordHash = passwordService.HashPassword("password123"),
                Role = "Coach",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var coachUser2 = new User
            {
                FirstName = "Sarah",
                LastName = "Williams",
                Email = "coach.williams@nextup.com",
                PasswordHash = passwordService.HashPassword("password456"),
                Role = "Coach",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create Player Users
            var playerUsers = new List<User>
            {
                new User { FirstName = "Jake", LastName = "Thompson", Email = "jake.thompson@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { FirstName = "Marcus", LastName = "Davis", Email = "marcus.davis@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { FirstName = "Alex", LastName = "Rodriguez", Email = "alex.rodriguez@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { FirstName = "Tyler", LastName = "Brown", Email = "tyler.brown@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { FirstName = "Jordan", LastName = "Miller", Email = "jordan.miller@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { FirstName = "Ryan", LastName = "Wilson", Email = "ryan.wilson@student.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            // Add all users
            context.Users.AddRange(new[] { coachUser1, coachUser2 });
            context.Users.AddRange(playerUsers);
            await context.SaveChangesAsync();

            // Create Teams
            var team1 = new Team
            {
                Name = "Lightning Bolts",
                Location = "Downtown High School",
                CoachId = coachUser1.UserId,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var team2 = new Team
            {
                Name = "Thunder Hawks",
                Location = "Riverside Academy",
                CoachId = coachUser2.UserId,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Teams.AddRange(team1, team2);
            await context.SaveChangesAsync();

            // Create Coach records
            var coach1 = new Coach
            {
                UserId = coachUser1.UserId,
                TeamId = team1.TeamId,
                ExperienceYears = 8,
                Specialty = "Offensive Strategy",
                Certification = "Level 3 Football Coach",
                Bio = "Experienced coach specializing in developing young talent and offensive plays.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var coach2 = new Coach
            {
                UserId = coachUser2.UserId,
                TeamId = team2.TeamId,
                ExperienceYears = 12,
                Specialty = "Defensive Tactics",
                Certification = "Master Football Coach",
                Bio = "Veteran coach known for building strong defensive lines and team discipline.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Coaches.AddRange(coach1, coach2);
            await context.SaveChangesAsync();

            // Create Players for Team 1 (Lightning Bolts)
            var team1Players = new List<Player>
            {
                new Player { UserId = playerUsers[0].UserId, TeamId = team1.TeamId, Position = "Quarterback", Age = 17, Height = "6'1\"", Weight = 185, JerseyNumber = 12, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Player { UserId = playerUsers[1].UserId, TeamId = team1.TeamId, Position = "Running Back", Age = 16, Height = "5'10\"", Weight = 175, JerseyNumber = 23, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Player { UserId = playerUsers[2].UserId, TeamId = team1.TeamId, Position = "Wide Receiver", Age = 17, Height = "6'0\"", Weight = 165, JerseyNumber = 88, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            // Create Players for Team 2 (Thunder Hawks)
            var team2Players = new List<Player>
            {
                new Player { UserId = playerUsers[3].UserId, TeamId = team2.TeamId, Position = "Linebacker", Age = 18, Height = "6'2\"", Weight = 195, JerseyNumber = 44, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Player { UserId = playerUsers[4].UserId, TeamId = team2.TeamId, Position = "Safety", Age = 17, Height = "5'11\"", Weight = 180, JerseyNumber = 21, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Player { UserId = playerUsers[5].UserId, TeamId = team2.TeamId, Position = "Offensive Line", Age = 18, Height = "6'4\"", Weight = 220, JerseyNumber = 77, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Players.AddRange(team1Players);
            context.Players.AddRange(team2Players);
            await context.SaveChangesAsync();

            // Create a sample game
            var game = new Game
            {
                HomeTeamId = team1.TeamId,
                AwayTeamId = team2.TeamId,
                GameDate = DateTime.UtcNow.AddDays(-7), // Game was last week
                Location = "Central Stadium",
                HomeScore = 21,
                AwayScore = 14,
                Status = "Completed",
                Season = "2024-25",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Games.Add(game);
            await context.SaveChangesAsync();

            // Create some player goals
            var playerGoals = new List<PlayerGoal>
            {
                new PlayerGoal { PlayerId = team1Players[0].PlayerId, GoalType = "Passing Yards", TargetValue = 2500, CurrentValue = 1200, Season = "2024-25", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PlayerGoal { PlayerId = team1Players[1].PlayerId, GoalType = "Rushing Yards", TargetValue = 1000, CurrentValue = 650, Season = "2024-25", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PlayerGoal { PlayerId = team2Players[0].PlayerId, GoalType = "Tackles", TargetValue = 80, CurrentValue = 45, Season = "2024-25", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.PlayerGoals.AddRange(playerGoals);
            await context.SaveChangesAsync();

            // Create some sample game stats
            var gameStats = new List<PlayerGameStats>
            {
                new PlayerGameStats
                {
                    PlayerId = team1Players[0].PlayerId, // QB
                    GameId = game.GameId,
                    PassingYards = 245,
                    PassingTouchdowns = 2,
                    Interceptions = 1,
                    RushingYards = 15,
                    MinutesPlayed = 58,
                    RecordedBy = coachUser1.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new PlayerGameStats
                {
                    PlayerId = team1Players[1].PlayerId, // RB
                    GameId = game.GameId,
                    RushingYards = 85,
                    RushingTouchdowns = 1,
                    ReceivingYards = 25,
                    Receptions = 3,
                    MinutesPlayed = 52,
                    RecordedBy = coachUser1.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.PlayerGameStats.AddRange(gameStats);
            await context.SaveChangesAsync();
        }
    }
}