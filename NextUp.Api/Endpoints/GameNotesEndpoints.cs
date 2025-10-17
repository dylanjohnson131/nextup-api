using Microsoft.EntityFrameworkCore;
using NextUp.Data;
using NextUp.Models;
namespace NextUp.Api.Endpoints;
public static class GameNotesEndpoints
{
    public static void MapGameNotesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/game-notes");
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var notes = await db.GameNotes
                .Include(n => n.Game)
                .Include(n => n.Coach)
                .ToListAsync();
            return Results.Ok(notes.Select(n => new
            {
                n.GameNoteId,
                Game = new { n.GameId, n.Game.GameDate, n.Game.Location },
                Coach = new { n.CoachId, n.Coach.Email },
                n.NoteType,
                n.Title,
                n.Content,
                n.CreatedAt
            }));
        });
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var n = await db.GameNotes
                .Include(x => x.Game)
                .Include(x => x.Coach)
                .FirstOrDefaultAsync(x => x.GameNoteId == id);
            if (n == null) return Results.NotFound(new { error = $"Game note with ID {id} not found." });
            return Results.Ok(new
            {
                n.GameNoteId,
                Game = new { n.GameId, n.Game.GameDate, n.Game.Location },
                Coach = new { n.CoachId, n.Coach.Email },
                n.NoteType,
                n.Title,
                n.Content,
                n.CreatedAt,
                n.UpdatedAt
            });
        });
    group.MapPost("/", async (GameNote request, NextUpDbContext db) =>
        {
            var game = await db.Games.FindAsync(request.GameId);
            var coach = await db.Users.FindAsync(request.CoachId);
            if (game == null || coach == null) return Results.BadRequest(new { error = "Game or Coach not found." });
            request.GameNoteId = 0;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;
            db.GameNotes.Add(request);
            await db.SaveChangesAsync();
            return Results.Created($"/api/game-notes/{request.GameNoteId}", new { request.GameNoteId });
        }).RequireAuthorization("CoachOnly");
    group.MapPut("/{id:int}", async (int id, GameNote update, NextUpDbContext db) =>
        {
            var n = await db.GameNotes.FindAsync(id);
            if (n == null) return Results.NotFound(new { error = $"Game note with ID {id} not found." });
            if (update.NoteType != null) n.NoteType = update.NoteType;
            if (update.Title != null) n.Title = update.Title;
            if (update.Content != null) n.Content = update.Content;
            n.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Game note updated", n.GameNoteId });
        }).RequireAuthorization("CoachOnly");
    group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var n = await db.GameNotes.FindAsync(id);
            if (n == null) return Results.NotFound(new { error = $"Game note with ID {id} not found." });
            db.GameNotes.Remove(n);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Game note {id} deleted." });
        }).RequireAuthorization("CoachOnly");
    }
}
