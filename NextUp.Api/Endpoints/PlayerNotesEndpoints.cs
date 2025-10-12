using Microsoft.EntityFrameworkCore;
using NextUp.Data;
using NextUp.Models;

namespace NextUp.Api.Endpoints;

public static class PlayerNotesEndpoints
{
    public static void MapPlayerNotesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/player-notes");

        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var notes = await db.PlayerNotes
                .Include(n => n.Player).ThenInclude(p => p.User)
                .Include(n => n.Coach)
                .ToListAsync();

            return Results.Ok(notes.Select(n => new
            {
                n.PlayerNoteId,
                Player = new { n.PlayerId, Name = n.Player != null ? $"{n.Player.User?.FirstName} {n.Player.User?.LastName}" : null },
                Coach = new { n.CoachId, n.Coach.Email },
                n.Content,
                n.IsPrivate,
                n.CreatedAt
            }));
        });

        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var n = await db.PlayerNotes
                .Include(x => x.Player).ThenInclude(p => p.User)
                .Include(x => x.Coach)
                .FirstOrDefaultAsync(x => x.PlayerNoteId == id);
            if (n == null) return Results.NotFound(new { error = $"Player note with ID {id} not found." });
            return Results.Ok(new
            {
                n.PlayerNoteId,
                Player = new { n.PlayerId, Name = n.Player != null ? $"{n.Player.User?.FirstName} {n.Player.User?.LastName}" : null },
                Coach = new { n.CoachId, n.Coach.Email },
                n.Content,
                n.IsPrivate,
                n.CreatedAt,
                n.UpdatedAt
            });
        });

        group.MapPost("/", async (PlayerNote request, NextUpDbContext db) =>
        {
            // Minimal validation
            var player = await db.Players.FindAsync(request.PlayerId);
            var coach = await db.Users.FindAsync(request.CoachId);
            if (player == null || coach == null) return Results.BadRequest(new { error = "Player or Coach not found." });

            request.PlayerNoteId = 0;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            db.PlayerNotes.Add(request);
            await db.SaveChangesAsync();
            return Results.Created($"/api/player-notes/{request.PlayerNoteId}", new { request.PlayerNoteId });
        });

        group.MapPut("/{id:int}", async (int id, PlayerNote update, NextUpDbContext db) =>
        {
            var n = await db.PlayerNotes.FindAsync(id);
            if (n == null) return Results.NotFound(new { error = $"Player note with ID {id} not found." });

            if (update.Content != null) n.Content = update.Content;
            n.IsPrivate = update.IsPrivate;
            n.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Player note updated", n.PlayerNoteId });
        });

        group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var n = await db.PlayerNotes.FindAsync(id);
            if (n == null) return Results.NotFound(new { error = $"Player note with ID {id} not found." });
            db.PlayerNotes.Remove(n);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Player note {id} deleted." });
        });
    }
}
