using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
namespace NextUp.Api.Endpoints
{
    public static class TeamEndpoints
    {
        public static void MapTeamEndpoints(this WebApplication app)
        {
            var teamGroup = app.MapGroup("/api/teams");
            teamGroup.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
            {
                var team = await db.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.TeamId == id);
                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });
                db.Teams.Remove(team);
                await db.SaveChangesAsync();
                return Results.Ok(new { message = $"Team {id} deleted." });
            });
            teamGroup.MapGet("/", async (NextUpDbContext db) =>
            {
                var teams = await db.Teams
                    .Include(team => team.Coach)
                        .ThenInclude(user => user.Coach)
                    .Include(team => team.Players)
                        .ThenInclude(player => player.User)
                    .ToListAsync();
                return Results.Ok(teams.Select(t => new
                {
                    t.TeamId,
                    t.Name,
                    t.Location,
                    t.IsPublic,
                    t.CreatedAt,
                    Coach = t.Coach != null ? new
                    {
                        CoachId = t.Coach.Coach?.CoachId,
                        Name = $"{t.Coach.FirstName} {t.Coach.LastName}",
                        ExperienceYears = t.Coach.Coach?.ExperienceYears ?? 0,
                        Specialty = t.Coach.Coach?.Specialty
                    } : null,
                    PlayerCount = t.Players?.Count ?? 0,
                    Players = (t.Players?.Select(p => (object)new
                    {
                        p.PlayerId,
                        Name = $"{p.User?.FirstName} {p.User?.LastName}",
                        p.Position,
                        p.JerseyNumber,
                        p.Age
                    })) ?? Array.Empty<object>()
                }));
            });
            teamGroup.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
            {
                var team = await db.Teams
                    .Include(t => t.Coach)
                        .ThenInclude(u => u.Coach)
                    .Include(t => t.Players)
                        .ThenInclude(p => p.User)
                    .Include(t => t.HomeGames)
                    .Include(t => t.AwayGames)
                    .FirstOrDefaultAsync(t => t.TeamId == id);
                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });
                return Results.Ok(new
                {
                    team.TeamId,
                    team.Name,
                    team.Location,
                    team.IsPublic,
                    team.CreatedAt,
                    Coach = team.Coach != null ? new
                    {
                        CoachId = team.Coach.Coach?.CoachId,
                        Name = $"{team.Coach.FirstName} {team.Coach.LastName}",
                        Email = team.Coach.Email,
                        ExperienceYears = team.Coach.Coach?.ExperienceYears ?? 0,
                        Specialty = team.Coach.Coach?.Specialty,
                        Certification = team.Coach.Coach?.Certification,
                        Bio = team.Coach.Coach?.Bio
                    } : null,
                    Players = (team.Players?.Select(p => (object)new
                    {
                        p.PlayerId,
                        Name = $"{p.User?.FirstName} {p.User?.LastName}",
                        Email = p.User?.Email,
                        p.Position,
                        p.JerseyNumber,
                        p.Age,
                        p.Height,
                        p.Weight
                    })) ?? Array.Empty<object>(),
                    Stats = new
                    {
                        TotalPlayers = team.Players?.Count ?? 0,
                        HomeGames = team.HomeGames?.Count ?? 0,
                        AwayGames = team.AwayGames?.Count ?? 0
                    }
                });
            });
            teamGroup.MapGet("/debug", async (NextUpDbContext db) =>
            {
                var teams = await db.Teams.Select(t => new { t.TeamId, t.Name }).ToListAsync();
                return Results.Ok(teams);
            });
            teamGroup.MapGet("/seed-missing", async (NextUpDbContext db, IPasswordService passwordService) =>
            {
                var stormRiders = await db.Teams.FirstOrDefaultAsync(t => t.Name == "Storm Riders");
                var eaglesFC = await db.Teams.FirstOrDefaultAsync(t => t.Name == "Eagles FC");
                if (stormRiders != null && eaglesFC != null)
                {
                    return Results.Ok(new { message = "Teams already exist" });
                }
                var addedTeams = new List<string>();
                if (stormRiders == null)
                {
                    var stormCoach = new User
                    {
                        FirstName = "Mark",
                        LastName = "Stevens",
                        Email = "coach.stevens@stormriders.com",
                        PasswordHash = passwordService.HashPassword("password789"),
                        Role = "Coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Users.Add(stormCoach);
                    await db.SaveChangesAsync();
                    var stormTeam = new Team
                    {
                        Name = "Storm Riders",
                        Location = "Westside Sports Complex",
                        CoachId = stormCoach.UserId,
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Teams.Add(stormTeam);
                    await db.SaveChangesAsync();
                    var stormPlayers = new List<User>
                    {
                        new User { FirstName = "Derek", LastName = "Wilson", Email = "derek.wilson@stormriders.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Chris", LastName = "Anderson", Email = "chris.anderson@stormriders.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Mike", LastName = "Taylor", Email = "mike.taylor@stormriders.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Josh", LastName = "Martinez", Email = "josh.martinez@stormriders.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    };
                    db.Users.AddRange(stormPlayers);
                    await db.SaveChangesAsync();
                    var stormPlayerRecords = new List<Player>
                    {
                        new Player { UserId = stormPlayers[0].UserId, TeamId = stormTeam.TeamId, Position = "Quarterback", Age = 18, Height = "6'2\"", Weight = 200, JerseyNumber = 12, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = stormPlayers[1].UserId, TeamId = stormTeam.TeamId, Position = "Wide Receiver", Age = 17, Height = "6'0\"", Weight = 185, JerseyNumber = 84, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = stormPlayers[2].UserId, TeamId = stormTeam.TeamId, Position = "Running Back", Age = 16, Height = "5'9\"", Weight = 180, JerseyNumber = 23, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = stormPlayers[3].UserId, TeamId = stormTeam.TeamId, Position = "Linebacker", Age = 18, Height = "6'1\"", Weight = 195, JerseyNumber = 55, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    };
                    db.Players.AddRange(stormPlayerRecords);
                    addedTeams.Add("Storm Riders");
                }
                if (eaglesFC == null)
                {
                    var eaglesCoach = new User
                    {
                        FirstName = "Lisa",
                        LastName = "Martinez",
                        Email = "coach.martinez@eaglesfc.com",
                        PasswordHash = passwordService.HashPassword("password101"),
                        Role = "Coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Users.Add(eaglesCoach);
                    await db.SaveChangesAsync();
                    var eaglesTeam = new Team
                    {
                        Name = "Eagles FC",
                        Location = "Central Athletic Center",
                        CoachId = eaglesCoach.UserId,
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Teams.Add(eaglesTeam);
                    await db.SaveChangesAsync();
                    var eaglesPlayers = new List<User>
                    {
                        new User { FirstName = "Kevin", LastName = "Garcia", Email = "kevin.garcia@eaglesfc.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Danny", LastName = "Lopez", Email = "danny.lopez@eaglesfc.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Sam", LastName = "White", Email = "sam.white@eaglesfc.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new User { FirstName = "Tony", LastName = "Clark", Email = "tony.clark@eaglesfc.com", PasswordHash = passwordService.HashPassword("player123"), Role = "Player", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    };
                    db.Users.AddRange(eaglesPlayers);
                    await db.SaveChangesAsync();
                    var eaglesPlayerRecords = new List<Player>
                    {
                        new Player { UserId = eaglesPlayers[0].UserId, TeamId = eaglesTeam.TeamId, Position = "Quarterback", Age = 17, Height = "5'11\"", Weight = 175, JerseyNumber = 8, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = eaglesPlayers[1].UserId, TeamId = eaglesTeam.TeamId, Position = "Wide Receiver", Age = 18, Height = "6'0\"", Weight = 180, JerseyNumber = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = eaglesPlayers[2].UserId, TeamId = eaglesTeam.TeamId, Position = "Safety", Age = 17, Height = "5'11\"", Weight = 175, JerseyNumber = 26, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                        new Player { UserId = eaglesPlayers[3].UserId, TeamId = eaglesTeam.TeamId, Position = "Defensive End", Age = 18, Height = "6'4\"", Weight = 215, JerseyNumber = 99, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                    };
                    db.Players.AddRange(eaglesPlayerRecords);
                    addedTeams.Add("Eagles FC");
                }
                await db.SaveChangesAsync();
                return Results.Ok(new { message = $"Successfully added teams: {string.Join(", ", addedTeams)}" });
            });
            teamGroup.MapGet("/{identifier}/overview", async (string identifier, NextUpDbContext db) =>
            {
                Console.WriteLine($"DEBUG: Looking for team with identifier: '{identifier}'");
                Team team = null;
                if (int.TryParse(identifier, out int id))
                {
                    Console.WriteLine($"DEBUG: Trying to find team by ID: {id}");
                    team = await db.Teams
                        .Include(t => t.Coach)
                            .ThenInclude(u => u.Coach)
                        .Include(t => t.Players)
                            .ThenInclude(p => p.User)
                        .Include(t => t.HomeGames)
                        .Include(t => t.AwayGames)
                        .FirstOrDefaultAsync(t => t.TeamId == id);
                }
                if (team == null)
                {
                    var normalizedIdentifier = identifier.Replace("-", " ").ToLower();
                    Console.WriteLine($"DEBUG: Trying to find team by name with normalized identifier: '{normalizedIdentifier}'");
                    var allTeamNames = await db.Teams.Select(t => t.Name).ToListAsync();
                    Console.WriteLine($"DEBUG: Available teams: {string.Join(", ", allTeamNames)}");
                    team = await db.Teams
                        .Include(t => t.Coach)
                            .ThenInclude(u => u.Coach)
                        .Include(t => t.Players)
                            .ThenInclude(p => p.User)
                        .Include(t => t.HomeGames)
                        .Include(t => t.AwayGames)
                        .FirstOrDefaultAsync(t => t.Name.ToLower().Contains(normalizedIdentifier) || 
                                                 t.Name.ToLower().Replace(" ", "-") == identifier);
                }
                if (team == null)
                {
                    Console.WriteLine($"DEBUG: Team '{identifier}' not found after all search attempts");
                    return Results.NotFound(new { error = $"Team '{identifier}' not found." });
                }
                Console.WriteLine($"DEBUG: Found team: {team.Name} (ID: {team.TeamId})");
                var playersWithStats = await db.PlayerGameStats
                    .Include(pgs => pgs.Player)
                        .ThenInclude(p => p.User)
                    .Where(pgs => pgs.Player.TeamId == team.TeamId)
                    .GroupBy(pgs => pgs.Player)
                    .Select(g => new
                    {
                        Player = g.Key,
                        TotalTouchdowns = g.Sum(pgs => pgs.PassingTouchdowns + pgs.RushingTouchdowns + pgs.ReceivingTouchdowns),
                        TotalYards = g.Sum(pgs => pgs.PassingYards + pgs.RushingYards + pgs.ReceivingYards),
                        TotalTackles = g.Sum(pgs => pgs.Tackles),
                        GamesPlayed = g.Count()
                    })
                    .OrderByDescending(p => p.TotalTouchdowns)
                    .Take(5)
                    .ToListAsync();
                var recentGames = await db.Games
                    .Where(g => g.HomeTeamId == team.TeamId || g.AwayTeamId == team.TeamId)
                    .OrderByDescending(g => g.GameDate)
                    .Take(5)
                    .Include(g => g.HomeTeam)
                    .Include(g => g.AwayTeam)
                    .ToListAsync();
                int wins = 0, losses = 0, ties = 0;
                foreach (var game in recentGames)
                {
                    bool isHome = game.HomeTeamId == team.TeamId;
                    int teamScore = isHome ? game.HomeScore : game.AwayScore;
                    int opponentScore = isHome ? game.AwayScore : game.HomeScore;
                    if (teamScore > opponentScore) wins++;
                    else if (teamScore < opponentScore) losses++;
                    else ties++;
                }
                return Results.Ok(new
                {
                    Team = new
                    {
                        team.TeamId,
                        team.Name,
                        team.Location,
                        Wins = wins,
                        Losses = losses,
                        Ties = ties
                    },
                    Roster = team.Players.Select(p => new
                    {
                        p.PlayerId,
                        Name = $"{p.User.FirstName} {p.User.LastName}",
                        p.Position,
                        p.Age,
                        p.JerseyNumber,
                        Email = p.User.Email
                    }).ToList(),
                    TopPerformers = playersWithStats.Select((p, index) => new
                    {
                        p.Player.PlayerId,
                        Name = $"{p.Player.User.FirstName} {p.Player.User.LastName}",
                        p.Player.Position,
                        Stat = p.TotalTouchdowns.ToString(),
                        StatType = "Touchdowns"
                    }).ToList(),
                    Analysis = new
                    {
                        Strengths = GenerateTeamStrengths(team, playersWithStats),
                        Weaknesses = GenerateTeamWeaknesses(team, playersWithStats)
                    },
                    RecentGames = recentGames.Select(g => new
                    {
                        Date = g.GameDate.ToString("MMM dd, yyyy"),
                        Opponent = g.HomeTeamId == team.TeamId ? g.AwayTeam.Name : g.HomeTeam.Name,
                        Score = g.HomeTeamId == team.TeamId ? $"{g.HomeScore}-{g.AwayScore}" : $"{g.AwayScore}-{g.HomeScore}",
                        Result = g.HomeTeamId == team.TeamId 
                                    ? (g.HomeScore > g.AwayScore ? "W" : g.HomeScore < g.AwayScore ? "L" : "T")
                                    : (g.AwayScore > g.HomeScore ? "W" : g.AwayScore < g.HomeScore ? "L" : "T")
                    }).ToList()
                });
            });
            teamGroup.MapPost("/", async (CreateTeamRequest request, NextUpDbContext db, IPasswordService passwordService) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Location))
                {
                    return Results.BadRequest(new { error = "Team name and location are required." });
                }
                var coachUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.CoachEmail);
                if (coachUser == null)
                {
                    if (string.IsNullOrWhiteSpace(request.CoachFirstName) || string.IsNullOrWhiteSpace(request.CoachLastName))
                    {
                        return Results.BadRequest(new { error = "Coach first name and last name are required for new coaches." });
                    }
                    coachUser = new User
                    {
                        FirstName = request.CoachFirstName,
                        LastName = request.CoachLastName,
                        Email = request.CoachEmail,
                        PasswordHash = passwordService.HashPassword(request.CoachPassword ?? "defaultpassword123"),
                        Role = "Coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Users.Add(coachUser);
                    await db.SaveChangesAsync();
                }
                var team = new Team
                {
                    Name = request.Name,
                    Location = request.Location,
                    CoachId = coachUser.UserId,
                    IsPublic = request.IsPublic,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Teams.Add(team);
                await db.SaveChangesAsync();
                var existingCoach = await db.Coaches.FirstOrDefaultAsync(c => c.UserId == coachUser.UserId);
                if (existingCoach == null)
                {
                    var coach = new Coach
                    {
                        UserId = coachUser.UserId,
                        TeamId = team.TeamId,
                        ExperienceYears = request.CoachExperience ?? 1,
                        Specialty = request.CoachSpecialty ?? "General Coaching",
                        Certification = request.CoachCertification ?? "Basic",
                        Bio = request.CoachBio ?? "Experienced football coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Coaches.Add(coach);
                    await db.SaveChangesAsync();
                }
                return Results.Created($"/api/teams/{team.TeamId}", new 
                { 
                    team.TeamId, 
                    team.Name, 
                    team.Location, 
                    team.IsPublic,
                    CoachName = $"{coachUser.FirstName} {coachUser.LastName}",
                    CoachEmail = coachUser.Email
                });
            }).RequireAuthorization("AthleticDirectorOnly");
            teamGroup.MapPut("/{id:int}", async (int id, UpdateTeamRequest request, NextUpDbContext db) =>
            {
                var team = await db.Teams.FindAsync(id);
                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });
                if (!string.IsNullOrWhiteSpace(request.Name))
                    team.Name = request.Name;
                if (!string.IsNullOrWhiteSpace(request.Location))
                    team.Location = request.Location;
                if (request.IsPublic.HasValue)
                    team.IsPublic = request.IsPublic.Value;
                team.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return Results.Ok(new 
                { 
                    team.TeamId, 
                    team.Name, 
                    team.Location, 
                    team.IsPublic,
                    UpdatedAt = team.UpdatedAt
                });
            }).RequireAuthorization("CoachOnly");
            teamGroup.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
            {
                var team = await db.Teams
                    .Include(t => t.Players)
                    .Include(t => t.HomeGames)
                    .Include(t => t.AwayGames)
                    .FirstOrDefaultAsync(t => t.TeamId == id);
                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });
                if (team.HomeGames.Any() || team.AwayGames.Any())
                {
                    return Results.BadRequest(new 
                    { 
                        error = "Cannot delete team with existing games. Please remove all games first.",
                        homeGames = team.HomeGames.Count,
                        awayGames = team.AwayGames.Count
                    });
                }
                if (team.Players.Any())
                {
                    return Results.BadRequest(new 
                    { 
                        error = "Cannot delete team with players. Please remove all players first.",
                        playerCount = team.Players.Count
                    });
                }
                db.Teams.Remove(team);
                await db.SaveChangesAsync();
                return Results.Ok(new { message = $"Team '{team.Name}' has been deleted successfully." });
            }).RequireAuthorization("CoachOnly");
        }
        private static List<string> GenerateTeamStrengths(Team team, IEnumerable<dynamic> playerStats)
        {
            var strengths = new List<string>();
            if (team.Players.Count >= 10)
                strengths.Add("Deep roster with good depth");
            if (playerStats.Any())
            {
                var topScorer = playerStats.FirstOrDefault();
                if (topScorer?.TotalTouchdowns > 5)
                    strengths.Add("Strong offensive production");
                var avgAge = team.Players.Average(p => p.Age ?? 18);
                if (avgAge < 17)
                    strengths.Add("Young, energetic team");
                else if (avgAge > 19)
                    strengths.Add("Experienced, mature players");
            }
            var positions = team.Players.GroupBy(p => p.Position).Select(g => g.Key).ToList();
            if (positions.Count >= 4)
                strengths.Add("Well-balanced positional coverage");
            return strengths.Any() ? strengths : new List<string> { "Solid fundamentals", "Good team chemistry" };
        }
        private static List<string> GenerateTeamWeaknesses(Team team, IEnumerable<dynamic> playerStats)
        {
            var weaknesses = new List<string>();
            if (team.Players.Count < 8)
                weaknesses.Add("Limited roster depth");
            if (playerStats.Any())
            {
                var totalGames = playerStats.FirstOrDefault()?.GamesPlayed ?? 0;
                if (totalGames < 3)
                    weaknesses.Add("Limited game experience");
            }
            var positions = team.Players.GroupBy(p => p.Position).ToList();
            if (positions.Any(g => g.Count() == 1))
                weaknesses.Add("Thin coverage in key positions");
            if (!playerStats.Any())
                weaknesses.Add("Inconsistent statistical tracking");
            return weaknesses.Any() ? weaknesses : new List<string> { "Areas for improvement in conditioning", "Could benefit from more strategic depth" };
        }
    }
}
