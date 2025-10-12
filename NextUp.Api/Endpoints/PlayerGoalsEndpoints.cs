using Microsoft.EntityFrameworkCore;
using NextUp.Data;
using NextUp.Models;

namespace NextUp.Api.Endpoints;

public static class PlayerGoalsEndpoints
{
    public static void MapPlayerGoalsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/player-goals");

        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var goals = await db.PlayerGoals
                .Include(g => g.Player).ThenInclude(p => p.User)
                .ToListAsync();
            return Results.Ok(goals.Select(g => new
            {
                g.PlayerGoalId,
                Player = new { g.PlayerId, Name = g.Player != null ? $"{g.Player.User?.FirstName} {g.Player.User?.LastName}" : null },
                g.GoalType,
                g.TargetValue,
                g.CurrentValue,
                g.Season,
                g.CreatedAt
            }));
        });

        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var g = await db.PlayerGoals
                .Include(x => x.Player).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(x => x.PlayerGoalId == id);
            if (g == null) return Results.NotFound(new { error = $"Player goal with ID {id} not found." });
            return Results.Ok(new
            {
                g.PlayerGoalId,
                Player = new { g.PlayerId, Name = g.Player != null ? $"{g.Player.User?.FirstName} {g.Player.User?.LastName}" : null },
                g.GoalType,
                g.TargetValue,
                g.CurrentValue,
                g.Season,
                g.CreatedAt,
                g.UpdatedAt
            });
        });

        group.MapPost("/", async (PlayerGoal request, NextUpDbContext db) =>
        {
            var player = await db.Players.FindAsync(request.PlayerId);
            if (player == null) return Results.BadRequest(new { error = "Player not found." });

            request.PlayerGoalId = 0;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            db.PlayerGoals.Add(request);
            await db.SaveChangesAsync();
            return Results.Created($"/api/player-goals/{request.PlayerGoalId}", new { request.PlayerGoalId });
        });

        group.MapPut("/{id:int}", async (int id, PlayerGoal update, NextUpDbContext db) =>
        {
            var g = await db.PlayerGoals.FindAsync(id);
            if (g == null) return Results.NotFound(new { error = $"Player goal with ID {id} not found." });

            if (update.GoalType != null) g.GoalType = update.GoalType;
            if (update.TargetValue != 0) g.TargetValue = update.TargetValue; // simple update; 0 means leave as-is if they didn't intend
            g.CurrentValue = update.CurrentValue; // can be 0
            if (update.Season != null) g.Season = update.Season;
            g.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Player goal updated", g.PlayerGoalId });
        });

        group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var g = await db.PlayerGoals.FindAsync(id);
            if (g == null) return Results.NotFound(new { error = $"Player goal with ID {id} not found." });
            db.PlayerGoals.Remove(g);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Player goal {id} deleted." });
        });
    }
}
