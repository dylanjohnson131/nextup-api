using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
using System.Security.Claims;
namespace NextUp.Api.Endpoints;
public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/players");
        group.MapGet("/me", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var player = await db.Players
                .Include(p => p.User)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (player == null)
                return Results.NotFound(new { error = "Player profile not found for current user." });
            return Results.Ok(new
            {
                player.PlayerId,
                Name = $"{player.User.FirstName} {player.User.LastName}",
                Email = player.User.Email,
                player.Position,
                player.Age,
                player.Height,
                player.Weight,
                player.JerseyNumber,
                Team = new { player.Team.TeamId, player.Team.Name, player.Team.Location }
            });
        });
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var players = await db.Players
                .Include(p => p.User)
                .Include(p => p.Team)
                .ToListAsync();
            return Results.Ok(players.Select(p => new
            {
                p.PlayerId,
                Name = $"{p.User.FirstName} {p.User.LastName}",
                Email = p.User.Email,
                p.Position,
                p.Age,
                p.Height,
                p.Weight,
                p.JerseyNumber,
                Team = new { p.Team.TeamId, p.Team.Name, p.Team.Location }
            }));
        });
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var player = await db.Players
                .Include(p => p.User)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.PlayerId == id);
            if (player == null)
                return Results.NotFound(new { error = $"Player with ID {id} not found." });
            return Results.Ok(new
            {
                player.PlayerId,
                Name = $"{player.User.FirstName} {player.User.LastName}",
                Email = player.User.Email,
                player.Position,
                player.Age,
                player.Height,
                player.Weight,
                player.JerseyNumber,
                Team = new { player.Team.TeamId, player.Team.Name, player.Team.Location }
            });
        });
        group.MapPost("/", async (CreatePlayerRequest request, NextUpDbContext db, IPasswordService passwordService) =>
        {
            var team = await db.Teams.FindAsync(request.TeamId);
            if (team == null)
                return Results.BadRequest(new { error = $"Team with ID {request.TeamId} does not exist." });
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser == null)
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return Results.BadRequest(new { error = "First name and last name are required for new user." });
                existingUser = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = passwordService.HashPassword(request.Password ?? "defaultpassword123"),
                    Role = "Player",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Users.Add(existingUser);
                await db.SaveChangesAsync();
            }
            var player = new Player
            {
                UserId = existingUser.UserId,
                TeamId = request.TeamId,
                Position = request.Position,
                Age = request.Age,
                Height = request.Height ?? string.Empty,
                Weight = request.Weight,
                JerseyNumber = request.JerseyNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Players.Add(player);
            await db.SaveChangesAsync();
            return Results.Created($"/api/players/{player.PlayerId}", new
            {
                player.PlayerId,
                Name = $"{existingUser.FirstName} {existingUser.LastName}",
                Email = existingUser.Email,
                player.Position,
                player.Age,
                player.Height,
                player.Weight,
                player.JerseyNumber,
                Team = new { team.TeamId, team.Name, team.Location }
            });
        });
        group.MapPut("/{id:int}", async (int id, UpdatePlayerRequest request, NextUpDbContext db) =>
        {
            var player = await db.Players.FindAsync(id);
            if (player == null)
                return Results.NotFound(new { error = $"Player with ID {id} not found." });
            if (!string.IsNullOrWhiteSpace(request.Position))
                player.Position = request.Position;
            if (request.Age.HasValue)
                player.Age = request.Age;
            if (request.Height != null)
                player.Height = request.Height;
            if (request.Weight.HasValue)
                player.Weight = request.Weight;
            if (request.JerseyNumber.HasValue)
                player.JerseyNumber = request.JerseyNumber;
            player.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Player updated", player.PlayerId, player.Position, player.Age, player.Height, player.Weight, player.JerseyNumber, player.UpdatedAt });
        });
        group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var player = await db.Players
                .Include(p => p.Notes)
                .Include(p => p.Goals)
                .Include(p => p.GameStats)
                .FirstOrDefaultAsync(p => p.PlayerId == id);
            if (player == null)
                return Results.NotFound(new { error = $"Player with ID {id} not found." });
            if ((player.Notes?.Any() ?? false) || (player.Goals?.Any() ?? false) || (player.GameStats?.Any() ?? false))
            {
                return Results.BadRequest(new
                {
                    error = "Cannot delete player with related notes, goals, or game stats. Remove related records first.",
                    notes = player.Notes?.Count ?? 0,
                    goals = player.Goals?.Count ?? 0,
                    gameStats = player.GameStats?.Count ?? 0
                });
            }
            db.Players.Remove(player);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Player {id} deleted." });
        });
    }
}
