using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
namespace NextUp.Api.Endpoints;
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users.Select(u => new
            {
                u.UserId,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role,
                u.CreatedAt,
                u.UpdatedAt
            }));
        });
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var u = await db.Users
                .Include(x => x.Coach)
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.UserId == id);
            if (u == null) return Results.NotFound(new { error = $"User with ID {id} not found." });
            return Results.Ok(new
            {
                u.UserId,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role,
                Coach = u.Coach != null ? new { u.Coach.CoachId, u.Coach.TeamId, u.Coach.ExperienceYears } : null,
                PlayerCount = u.Players?.Count ?? 0,
                u.CreatedAt,
                u.UpdatedAt
            });
        });
        group.MapPost("/", async (CreateUserRequest request, NextUpDbContext db, IPasswordService passwordService) =>
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Role))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, Password, and Role are required." });
            }
            var exists = await db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists) return Results.Conflict(new { error = "A user with this email already exists." });
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.UserId}", new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role });
        });
        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, NextUpDbContext db, IPasswordService passwordService) =>
        {
            var u = await db.Users.FindAsync(id);
            if (u == null) return Results.NotFound(new { error = $"User with ID {id} not found." });
            if (request.Email != null && request.Email != u.Email)
            {
                var emailTaken = await db.Users.AnyAsync(x => x.Email == request.Email && x.UserId != id);
                if (emailTaken) return Results.Conflict(new { error = "Another user already uses this email." });
                u.Email = request.Email;
            }
            if (request.FirstName != null) u.FirstName = request.FirstName;
            if (request.LastName != null) u.LastName = request.LastName;
            if (request.Role != null) u.Role = request.Role;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                u.PasswordHash = passwordService.HashPassword(request.Password);
            }
            u.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "User updated", u.UserId, u.Email, u.Role, u.UpdatedAt });
        });
        group.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var u = await db.Users
                .Include(x => x.Players)
                .Include(x => x.Coach)
                .Include(x => x.RecordedStats)
                .Include(x => x.GameNotes)
                .Include(x => x.PlayerNotes)
                .FirstOrDefaultAsync(x => x.UserId == id);
            if (u == null) return Results.NotFound(new { error = $"User with ID {id} not found." });
            if ((u.Players?.Any() ?? false) || u.Coach != null || (u.RecordedStats?.Any() ?? false) || (u.GameNotes?.Any() ?? false) || (u.PlayerNotes?.Any() ?? false))
            {
                return Results.BadRequest(new
                {
                    error = "Cannot delete user with related players/coach/stats/notes. Remove or reassign related records first.",
                    players = u.Players?.Count ?? 0,
                    coach = u.Coach != null,
                    recordedStats = u.RecordedStats?.Count ?? 0,
                    gameNotes = u.GameNotes?.Count ?? 0,
                    playerNotes = u.PlayerNotes?.Count ?? 0
                });
            }
            db.Users.Remove(u);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"User {id} deleted." });
        });
    }
}
