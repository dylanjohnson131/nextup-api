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

        // GET /api/athletic-directors/me - Get current Athletic Director profile
        group.MapGet("/me", async (NextUpDbContext db, ClaimsPrincipal user) =>
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

        // PUT /api/athletic-directors/me - Update Athletic Director profile
        group.MapPut("/me", async (UpdateAthleticDirectorRequest request, NextUpDbContext db, ClaimsPrincipal user) =>
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

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var athleticDirector = await db.AthleticDirectors.FirstOrDefaultAsync(ad => ad.UserId == userId);

            if (athleticDirector == null)
            {
                return Results.NotFound(new { error = "Athletic Director profile not found." });
            }

            // Update fields if provided
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

        // GET /api/athletic-directors/dashboard - Get season overview
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

            // Simplified dashboard for now
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

        // Team Management Endpoints

        // GET /api/athletic-directors/teams - Get all teams
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
                .Select(t => new
                {
                    t.TeamId,
                    t.Name,
                    t.Location,
                    t.IsPublic,
                    t.CreatedAt
                })
                .ToListAsync();

            return Results.Ok(teams);
        });

        // POST /api/athletic-directors/teams - Create new team
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

            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Location))
            {
                return Results.BadRequest(new { error = "Name and Location are required." });
            }

            // Check for duplicate team name
            var existingTeam = await db.Teams.FirstOrDefaultAsync(t => t.Name == request.Name);
            if (existingTeam != null)
            {
                return Results.Conflict(new { error = "A team with this name already exists." });
            }

            var team = new Team
            {
                Name = request.Name,
                Location = request.Location,
                IsPublic = request.IsPublic,
                CoachId = 0, // Temporary placeholder - will be updated when coach is assigned
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Teams.Add(team);
            await db.SaveChangesAsync();

            return Results.Created($"/api/teams/{team.TeamId}", new
            {
                message = "Team created successfully.",
                TeamId = team.TeamId,
                team.Name,
                team.Location,
                team.IsPublic
            });
        });

        // PUT /api/athletic-directors/teams/{id} - Update team
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

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                // Check for duplicate name (excluding current team)
                var existingTeam = await db.Teams.FirstOrDefaultAsync(t => t.Name == request.Name && t.TeamId != id);
                if (existingTeam != null)
                {
                    return Results.Conflict(new { error = "A team with this name already exists." });
                }
                team.Name = request.Name;
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
                team.Location = request.Location;
            if (request.IsPublic.HasValue)
                team.IsPublic = request.IsPublic.Value;
            if (request.CoachId.HasValue)
            {
                // Validate coach exists and is available
                var coach = await db.Coaches.FirstOrDefaultAsync(c => c.CoachId == request.CoachId.Value);
                if (coach == null)
                {
                    return Results.BadRequest(new { error = "Coach not found." });
                }
                team.CoachId = request.CoachId.Value;
            }

            team.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Team updated successfully." });
        });

        // DELETE /api/athletic-directors/teams/{id} - Delete team
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
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return Results.NotFound(new { error = "Team not found." });
            }

            // Check if team has players or scheduled games
            if (team.Players.Any())
            {
                return Results.BadRequest(new { error = "Cannot delete team with active players. Please transfer players first." });
            }

            if (team.HomeGames.Any() || team.AwayGames.Any())
            {
                return Results.BadRequest(new { error = "Cannot delete team with scheduled games. Please cancel games first." });
            }

            db.Teams.Remove(team);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Team deleted successfully." });
        });

        // Game Management Endpoints

        // GET /api/athletic-directors/games - Get all games
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
                    IsCompleted = g.Status == "Completed",
                    g.CreatedAt
                })
                .ToListAsync();

            return Results.Ok(games);
        });

        // POST /api/athletic-directors/games - Create new game
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

            // Validate teams exist and are different
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

            // Validate game date is in the future
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

        // PUT /api/athletic-directors/games/{id} - Update game
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

            // Don't allow updates to completed games unless specifically changing status
            if (game.Status == "Completed" && string.IsNullOrWhiteSpace(request.Status))
            {
                return Results.BadRequest(new { error = "Cannot modify completed games." });
            }

            // Update fields if provided
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

            game.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Game updated successfully." });
        });

        // DELETE /api/athletic-directors/games/{id} - Delete game
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

            // Don't allow deletion of games with recorded stats
            if (game.PlayerStats.Any())
            {
                return Results.BadRequest(new { error = "Cannot delete game with recorded statistics." });
            }

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Game deleted successfully." });
        });
    }
}