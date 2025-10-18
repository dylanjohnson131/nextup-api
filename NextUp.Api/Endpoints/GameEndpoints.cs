using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Data;
using NextUp.Models;
namespace NextUp.Api.Endpoints;
public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/games");
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var games = await db.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .ToListAsync();
            return Results.Ok(games.Select(g => new
            {
                g.GameId,
                g.GameDate,
                g.Location,
                g.HomeScore,
                g.AwayScore,
                g.Status,
                g.Season,
                HomeTeam = new { g.HomeTeamId, g.HomeTeam.Name, g.HomeTeam.Location },
                AwayTeam = new { g.AwayTeamId, g.AwayTeam.Name, g.AwayTeam.Location }
            }));
        });
        group.MapGet("/upcoming/{teamId:int}", async (int teamId, NextUpDbContext db) =>
        {
            var upcomingGames = await db.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Where(g => (g.HomeTeamId == teamId || g.AwayTeamId == teamId)
                           && g.HomeTeamId != g.AwayTeamId // Exclude self-matches
                           && g.GameDate > DateTime.UtcNow
                           && g.Status == "Scheduled")
                .OrderBy(g => g.GameDate)
                .Take(5)
                .ToListAsync();
            return Results.Ok(upcomingGames.Select(g => new
            {
                g.GameId,
                g.GameDate,
                g.Location,
                g.Status,
                g.Season,
                HomeTeam = new { g.HomeTeamId, g.HomeTeam.Name, g.HomeTeam.Location },
                AwayTeam = new { g.AwayTeamId, g.AwayTeam.Name, g.AwayTeam.Location },
                IsHome = g.HomeTeamId == teamId,
                Opponent = g.HomeTeamId == teamId 
                    ? new { TeamId = g.AwayTeamId, g.AwayTeam.Name, g.AwayTeam.Location }
                    : new { TeamId = g.HomeTeamId, g.HomeTeam.Name, g.HomeTeam.Location }
            }));
        });
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var game = await db.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.PlayerStats)
                .Include(g => g.GameNotes)
                .FirstOrDefaultAsync(g => g.GameId == id);
            if (game == null)
                return Results.NotFound(new { error = $"Game with ID {id} not found." });
            return Results.Ok(new
            {
                game.GameId,
                game.GameDate,
                game.Location,
                game.HomeScore,
                game.AwayScore,
                game.Status,
                game.Season,
                HomeTeam = new { game.HomeTeamId, game.HomeTeam.Name, game.HomeTeam.Location },
                AwayTeam = new { game.AwayTeamId, game.AwayTeam.Name, game.AwayTeam.Location },
                StatsCount = game.PlayerStats?.Count ?? 0,
                NotesCount = game.GameNotes?.Count ?? 0
            });
        });
    group.MapPost("/", async (CreateGameRequest request, NextUpDbContext db) =>
        {
            if (request.HomeTeamId == request.AwayTeamId)
                return Results.BadRequest(new { error = "HomeTeamId and AwayTeamId must be different." });
            var home = await db.Teams.FindAsync(request.HomeTeamId);
            var away = await db.Teams.FindAsync(request.AwayTeamId);
            if (home == null || away == null)
                return Results.BadRequest(new { error = "Home or Away team not found." });
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
                game.GameId,
                game.GameDate,
                game.Location,
                game.Season,
                HomeTeam = new { home.TeamId, home.Name },
                AwayTeam = new { away.TeamId, away.Name }
            });
        }).RequireAuthorization("CoachOnly");
    group.MapPut("/{id:int}", async (int id, UpdateGameRequest request, NextUpDbContext db) =>
        {
            var game = await db.Games.FindAsync(id);
            if (game == null)
                return Results.NotFound(new { error = $"Game with ID {id} not found." });
            if (request.GameDate.HasValue) game.GameDate = request.GameDate.Value;
            if (request.Location != null) game.Location = request.Location;
            if (request.HomeScore.HasValue) game.HomeScore = request.HomeScore.Value;
            if (request.AwayScore.HasValue) game.AwayScore = request.AwayScore.Value;
            if (request.Status != null) game.Status = request.Status;
            game.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Game updated", game.GameId, game.GameDate, game.Location, game.HomeScore, game.AwayScore, game.Status });
        }).RequireAuthorization("CoachOnly");
    group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var game = await db.Games
                .Include(g => g.PlayerStats)
                .Include(g => g.GameNotes)
                .FirstOrDefaultAsync(g => g.GameId == id);
            if (game == null)
                return Results.NotFound(new { error = $"Game with ID {id} not found." });
            if ((game.PlayerStats?.Any() ?? false) || (game.GameNotes?.Any() ?? false))
            {
                return Results.BadRequest(new { error = "Cannot delete game with related stats or notes. Remove them first.", stats = game.PlayerStats?.Count ?? 0, notes = game.GameNotes?.Count ?? 0 });
            }
            db.Games.Remove(game);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Game {id} deleted." });
        }).RequireAuthorization("CoachOnly");
    }
}
