using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Data;
using NextUp.Models;

namespace NextUp.Api.Endpoints;

public static class StatsEndpoints
{
    public static void MapStatsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/stats");

        // GET /api/stats
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var stats = await db.PlayerGameStats
                .Include(s => s.Player).ThenInclude(p => p.User)
                .Include(s => s.Game)
                .ToListAsync();

            return Results.Ok(stats.Select(s => new
            {
                s.PlayerGameStatsId,
                Player = new { s.PlayerId, Name = s.Player != null ? $"{s.Player.User?.FirstName} {s.Player.User?.LastName}" : null },
                Game = new { s.GameId, s.Game.GameDate, s.Game.Location },
                s.PassingYards,
                s.PassingTouchdowns,
                s.Interceptions,
                s.RushingYards,
                s.RushingTouchdowns,
                s.ReceivingYards,
                s.ReceivingTouchdowns,
                s.Receptions,
                s.Tackles,
                s.Assists,
                s.Sacks,
                s.ForcedFumbles,
                s.InterceptionsDef,
                s.FieldGoalsMade,
                s.FieldGoalsAttempted,
                s.ExtraPointsMade,
                s.ExtraPointsAttempted,
                s.MinutesPlayed
            }));
        });

        // GET /api/stats/{id}
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var s = await db.PlayerGameStats
                .Include(x => x.Player).ThenInclude(p => p.User)
                .Include(x => x.Game)
                .FirstOrDefaultAsync(x => x.PlayerGameStatsId == id);
            if (s == null)
                return Results.NotFound(new { error = $"Stats entry with ID {id} not found." });

            return Results.Ok(new
            {
                s.PlayerGameStatsId,
                Player = new { s.PlayerId, Name = s.Player != null ? $"{s.Player.User?.FirstName} {s.Player.User?.LastName}" : null },
                Game = new { s.GameId, s.Game.GameDate, s.Game.Location },
                s.PassingYards,
                s.PassingTouchdowns,
                s.Interceptions,
                s.RushingYards,
                s.RushingTouchdowns,
                s.ReceivingYards,
                s.ReceivingTouchdowns,
                s.Receptions,
                s.Tackles,
                s.Assists,
                s.Sacks,
                s.ForcedFumbles,
                s.InterceptionsDef,
                s.FieldGoalsMade,
                s.FieldGoalsAttempted,
                s.ExtraPointsMade,
                s.ExtraPointsAttempted,
                s.MinutesPlayed
            });
        });

        // POST /api/stats
    group.MapPost("/", async (CreatePlayerStatsRequest request, NextUpDbContext db) =>
        {
            var player = await db.Players.FindAsync(request.PlayerId);
            var game = await db.Games.FindAsync(request.GameId);
            if (player == null || game == null)
                return Results.BadRequest(new { error = "Player or Game not found." });

            var stats = new PlayerGameStats
            {
                PlayerId = request.PlayerId,
                GameId = request.GameId,
                PassingYards = request.PassingYards,
                PassingTouchdowns = request.PassingTouchdowns,
                Interceptions = request.Interceptions,
                RushingYards = request.RushingYards,
                RushingTouchdowns = request.RushingTouchdowns,
                ReceivingYards = request.ReceivingYards,
                ReceivingTouchdowns = request.ReceivingTouchdowns,
                Receptions = request.Receptions,
                Tackles = request.Tackles,
                Assists = request.Assists,
                Sacks = request.Sacks,
                ForcedFumbles = request.ForcedFumbles,
                InterceptionsDef = request.InterceptionsDef,
                FieldGoalsMade = request.FieldGoalsMade,
                FieldGoalsAttempted = request.FieldGoalsAttempted,
                ExtraPointsMade = request.ExtraPointsMade,
                ExtraPointsAttempted = request.ExtraPointsAttempted,
                MinutesPlayed = request.MinutesPlayed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.PlayerGameStats.Add(stats);
            await db.SaveChangesAsync();

            return Results.Created($"/api/stats/{stats.PlayerGameStatsId}", new { stats.PlayerGameStatsId, stats.PlayerId, stats.GameId });
        }).RequireAuthorization("CoachOnly");

        // PUT /api/stats/{id}
    group.MapPut("/{id:int}", async (int id, UpdatePlayerStatsRequest request, NextUpDbContext db) =>
        {
            var s = await db.PlayerGameStats.FindAsync(id);
            if (s == null)
                return Results.NotFound(new { error = $"Stats entry with ID {id} not found." });

            s.PassingYards = request.PassingYards ?? s.PassingYards;
            s.PassingTouchdowns = request.PassingTouchdowns ?? s.PassingTouchdowns;
            s.Interceptions = request.Interceptions ?? s.Interceptions;
            s.RushingYards = request.RushingYards ?? s.RushingYards;
            s.RushingTouchdowns = request.RushingTouchdowns ?? s.RushingTouchdowns;
            s.ReceivingYards = request.ReceivingYards ?? s.ReceivingYards;
            s.ReceivingTouchdowns = request.ReceivingTouchdowns ?? s.ReceivingTouchdowns;
            s.Receptions = request.Receptions ?? s.Receptions;
            s.Tackles = request.Tackles ?? s.Tackles;
            s.Assists = request.Assists ?? s.Assists;
            s.Sacks = request.Sacks ?? s.Sacks;
            s.ForcedFumbles = request.ForcedFumbles ?? s.ForcedFumbles;
            s.InterceptionsDef = request.InterceptionsDef ?? s.InterceptionsDef;
            s.FieldGoalsMade = request.FieldGoalsMade ?? s.FieldGoalsMade;
            s.FieldGoalsAttempted = request.FieldGoalsAttempted ?? s.FieldGoalsAttempted;
            s.ExtraPointsMade = request.ExtraPointsMade ?? s.ExtraPointsMade;
            s.ExtraPointsAttempted = request.ExtraPointsAttempted ?? s.ExtraPointsAttempted;
            s.MinutesPlayed = request.MinutesPlayed ?? s.MinutesPlayed;
            s.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Stats updated", s.PlayerGameStatsId });
        }).RequireAuthorization("CoachOnly");

        // DELETE /api/stats/{id}
    group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var s = await db.PlayerGameStats.FindAsync(id);
            if (s == null)
                return Results.NotFound(new { error = $"Stats entry with ID {id} not found." });
            db.PlayerGameStats.Remove(s);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Stats {id} deleted." });
        }).RequireAuthorization("CoachOnly");
    }
}
