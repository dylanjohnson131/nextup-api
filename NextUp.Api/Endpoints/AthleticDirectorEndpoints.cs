using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
using System.Security.Claims;
namespace NextUp.Api.Endpoints;
public static class AthleticDirectorEndpoints
{
    public static void MapAthleticDirectorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/athletic-directors");
        group.MapGet("/me", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != User.ATHLETIC_DIRECTOR_ROLE)
            {
                return Results.Forbid();
            }
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var athleticDirector = await db.AthleticDirectors
                .Include(ad => ad.User)
                .FirstOrDefaultAsync(ad => ad.UserId == userId);
            if (athleticDirector == null)
            {
                return Results.NotFound(new { error = "Athletic Director profile not found." });
            }
            return Results.Ok(new
            {
                athleticDirector.AthleticDirectorId,
                athleticDirector.UserId,
                User = new
                {
                    athleticDirector.User.FirstName,
                    athleticDirector.User.LastName,
                    athleticDirector.User.Email,
                    athleticDirector.User.Role
                },
                athleticDirector.Department,
                athleticDirector.ExperienceYears,
                athleticDirector.Certification,
                athleticDirector.Institution,
                athleticDirector.Bio,
                athleticDirector.CreatedAt,
                athleticDirector.UpdatedAt
            });
        });
        group.MapPut("/me", async (UpdateAthleticDirectorRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != User.ATHLETIC_DIRECTOR_ROLE)
            {
                return Results.Forbid();
            }
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var athleticDirector = await db.AthleticDirectors.FirstOrDefaultAsync(ad => ad.UserId == userId);
            if (athleticDirector == null)
            {
                return Results.NotFound(new { error = "Athletic Director profile not found." });
            }
            if (!string.IsNullOrWhiteSpace(request.Department))
                athleticDirector.Department = request.Department;
            if (request.ExperienceYears.HasValue)
                athleticDirector.ExperienceYears = request.ExperienceYears.Value;
            if (!string.IsNullOrWhiteSpace(request.Certification))
                athleticDirector.Certification = request.Certification;
            if (!string.IsNullOrWhiteSpace(request.Institution))
                athleticDirector.Institution = request.Institution;
            if (!string.IsNullOrWhiteSpace(request.Bio))
                athleticDirector.Bio = request.Bio;
            athleticDirector.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Athletic Director profile updated successfully." });
        });
        group.MapGet("/dashboard", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var totalTeams = await db.Teams.CountAsync();
            var totalGames = await db.Games.CountAsync();
            var completedGames = await db.Games.CountAsync(g => g.Status == "Completed");
            return Results.Ok(new
            {
                TotalTeams = totalTeams,
                TotalGames = totalGames,
                CompletedGames = completedGames,
                UpcomingGames = totalGames - completedGames
            });
        });
        group.MapGet("/teams", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var teams = await db.Teams
                .Include(t => t.Coach)
                .Select(t => new
                {
                    t.TeamId,
                    t.Name,
                    t.School,
                    t.Mascot,
                    t.Location,
                    t.City,
                    t.State,
                    t.Division,
                    t.Conference,
                    t.IsPublic,
                    t.CoachId,
                    Coach = t.Coach != null ? new {
                        CoachId = t.CoachId,
                        Name = $"{t.Coach.FirstName} {t.Coach.LastName}",
                        Email = t.Coach.Email
                    } : null,
                    t.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(teams);
        });
        group.MapPost("/teams", async (CreateTeamByAdRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Team name is required." });
            }
            Coach? coach = null;
            if (request.CoachId.HasValue)
            {
                coach = await db.Coaches.FirstOrDefaultAsync(c => c.CoachId == request.CoachId.Value);
                if (coach == null)
                {
                    return Results.BadRequest(new { error = "Selected coach not found." });
                }
            }
            var existingTeam = await db.Teams.FirstOrDefaultAsync(t => t.Name == request.Name);
            if (existingTeam != null)
            {
                return Results.Conflict(new { error = "A team with this name already exists." });
            }
            var team = new Team
            {
                Name = request.Name,
                School = request.School,
                Mascot = request.Mascot,
                Location = request.Location,
                City = request.City,
                State = request.State,
                Division = request.Division,
                Conference = request.Conference,
                IsPublic = request.IsPublic,
                CoachId = coach?.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Teams.Add(team);
            await db.SaveChangesAsync();
            if (coach != null)
            {
                coach.TeamId = team.TeamId;
                coach.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            return Results.Created($"/api/teams/{team.TeamId}", new
            {
                message = "Team created successfully.",
                TeamId = team.TeamId,
                team.Name,
                team.School,
                team.Mascot,
                team.Location,
                team.City,
                team.State,
                team.Division,
                team.Conference,
                team.IsPublic
            });
        });
        group.MapPut("/teams/{id:int}", async (int id, UpdateTeamByAdRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var team = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == id);
            if (team == null)
            {
                return Results.NotFound(new { error = "Team not found." });
            }
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var existingTeam = await db.Teams.FirstOrDefaultAsync(t => t.Name == request.Name && t.TeamId != id);
                if (existingTeam != null)
                {
                    return Results.Conflict(new { error = "A team with this name already exists." });
                }
                team.Name = request.Name;
            }
            if (!string.IsNullOrWhiteSpace(request.School))
                team.School = request.School;
            if (!string.IsNullOrWhiteSpace(request.Mascot))
                team.Mascot = request.Mascot;
            if (!string.IsNullOrWhiteSpace(request.Location))
                team.Location = request.Location;
            if (!string.IsNullOrWhiteSpace(request.City))
                team.City = request.City;
            if (!string.IsNullOrWhiteSpace(request.State))
                team.State = request.State;
            if (!string.IsNullOrWhiteSpace(request.Division))
                team.Division = request.Division;
            if (!string.IsNullOrWhiteSpace(request.Conference))
                team.Conference = request.Conference;
            if (request.IsPublic.HasValue)
                team.IsPublic = request.IsPublic.Value;
            if (request.CoachId.HasValue)
            {
                // Assign a new coach
                if (team.CoachId.HasValue)
                {
                    var previousCoach = await db.Coaches.FirstOrDefaultAsync(c => c.UserId == team.CoachId.Value);
                    if (previousCoach != null)
                    {
                        previousCoach.TeamId = null;
                        previousCoach.UpdatedAt = DateTime.UtcNow;
                    }
                }
                var newCoach = await db.Coaches.FirstOrDefaultAsync(c => c.CoachId == request.CoachId.Value);
                if (newCoach == null)
                {
                    return Results.BadRequest(new { error = "Coach not found." });
                }
                team.CoachId = newCoach.UserId;
                newCoach.TeamId = team.TeamId;
                newCoach.UpdatedAt = DateTime.UtcNow;
            }
            else if (request.CoachId == null)
            {
                // Unassign coach if CoachId is explicitly null
                if (team.CoachId.HasValue)
                {
                    var previousCoach = await db.Coaches.FirstOrDefaultAsync(c => c.UserId == team.CoachId.Value);
                    if (previousCoach != null)
                    {
                        previousCoach.TeamId = null;
                        previousCoach.UpdatedAt = DateTime.UtcNow;
                    }
                }
                team.CoachId = null;
            }
            team.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Team updated successfully." });
        });
        group.MapDelete("/teams/{id:int}", async (int id, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var team = await db.Teams
                .Include(t => t.Players)
                .Include(t => t.Coach)
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .FirstOrDefaultAsync(t => t.TeamId == id);
            if (team == null)
            {
                return Results.NotFound(new { error = "Team not found." });
            }
            if (team.Players.Any())
            {
                return Results.BadRequest(new { error = "Cannot delete team with active players. Please transfer players first." });
            }
            if (team.Coach != null)
            {
                return Results.BadRequest(new { error = "Cannot delete team with a coach assigned. Please unassign the coach first." });
            }
            if (team.HomeGames.Any() || team.AwayGames.Any())
            {
                return Results.BadRequest(new { error = "Cannot delete team with scheduled games. Please cancel games first." });
            }
            db.Teams.Remove(team);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Team deleted successfully." });
        });
        group.MapGet("/games", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var games = await db.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .OrderBy(g => g.GameDate)
                .Select(g => new
                {
                    g.GameId,
                    HomeTeam = new { g.HomeTeam.TeamId, g.HomeTeam.Name },
                    AwayTeam = new { g.AwayTeam.TeamId, g.AwayTeam.Name },
                    g.GameDate,
                    g.Location,
                    g.Week,
                    IsCompleted = g.Status == "Completed",
                    g.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(games);
        });
        group.MapPost("/games", async (CreateGameByAdRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            if (request.HomeTeamId == request.AwayTeamId)
            {
                return Results.BadRequest(new { error = "Home and away teams must be different." });
            }
            var homeTeam = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.HomeTeamId);
            var awayTeam = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.AwayTeamId);
            if (homeTeam == null || awayTeam == null)
            {
                return Results.BadRequest(new { error = "One or both teams not found." });
            }
            if (request.GameDate <= DateTime.UtcNow)
            {
                return Results.BadRequest(new { error = "Game date must be in the future." });
            }
            var game = new Game
            {
                HomeTeamId = request.HomeTeamId,
                AwayTeamId = request.AwayTeamId,
                GameDate = request.GameDate,
                Location = request.Location,
                Season = request.Season,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Games.Add(game);
            await db.SaveChangesAsync();
            return Results.Created($"/api/games/{game.GameId}", new
            {
                message = "Game created successfully.",
                GameId = game.GameId,
                HomeTeam = homeTeam.Name,
                AwayTeam = awayTeam.Name,
                game.GameDate,
                game.Location
            });
        });
        group.MapPut("/games/{id:int}", async (int id, UpdateGameByAdRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var game = await db.Games.FirstOrDefaultAsync(g => g.GameId == id);
            if (game == null)
            {
                return Results.NotFound(new { error = "Game not found." });
            }
            if (game.Status == "Completed" && string.IsNullOrWhiteSpace(request.Status))
            {
                return Results.BadRequest(new { error = "Cannot modify completed games." });
            }
            if (request.HomeTeamId.HasValue || request.AwayTeamId.HasValue)
            {
                var homeTeamId = request.HomeTeamId ?? game.HomeTeamId;
                var awayTeamId = request.AwayTeamId ?? game.AwayTeamId;
                if (homeTeamId == awayTeamId)
                {
                    return Results.BadRequest(new { error = "Home and away teams must be different." });
                }
                if (request.HomeTeamId.HasValue)
                {
                    var homeTeam = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.HomeTeamId.Value);
                    if (homeTeam == null)
                    {
                        return Results.BadRequest(new { error = "Home team not found." });
                    }
                    game.HomeTeamId = request.HomeTeamId.Value;
                }
                if (request.AwayTeamId.HasValue)
                {
                    var awayTeam = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.AwayTeamId.Value);
                    if (awayTeam == null)
                    {
                        return Results.BadRequest(new { error = "Away team not found." });
                    }
                    game.AwayTeamId = request.AwayTeamId.Value;
                }
            }
            if (request.GameDate.HasValue)
                game.GameDate = request.GameDate.Value;
            if (!string.IsNullOrWhiteSpace(request.Location))
                game.Location = request.Location;
            if (!string.IsNullOrWhiteSpace(request.Status))
                game.Status = request.Status;
            if (request.Week.HasValue)
                game.Week = request.Week;
            if (!string.IsNullOrWhiteSpace(request.Season))
                game.Season = request.Season;
            if (request.HomeScore.HasValue)
                game.HomeScore = request.HomeScore.Value;
            if (request.AwayScore.HasValue)
                game.AwayScore = request.AwayScore.Value;
            game.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Game updated successfully." });
        });
        group.MapDelete("/games/{id:int}", async (int id, NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "AthleticDirector")
            {
                return Results.Forbid();
            }
            var game = await db.Games
                .Include(g => g.GameNotes)
                .Include(g => g.PlayerStats)
                .FirstOrDefaultAsync(g => g.GameId == id);
            if (game == null)
            {
                return Results.NotFound(new { error = "Game not found." });
            }
            if (game.PlayerStats.Any())
            {
                db.PlayerGameStats.RemoveRange(game.PlayerStats);
            }
            if (game.GameNotes.Any())
            {
                db.GameNotes.RemoveRange(game.GameNotes);
            }
            db.Games.Remove(game);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Game deleted successfully." });
        });
    }
}
