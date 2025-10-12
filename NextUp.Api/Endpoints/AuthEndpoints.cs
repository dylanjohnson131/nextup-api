using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
using System.Security.Claims;

namespace NextUp.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        // POST /auth/register/player
        group.MapPost("/register/player", async (RegisterPlayerRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, and Password are required." });
            }

            // Enforce unique email
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return Results.Conflict(new { error = "A user with this email already exists." });
            }

            // Validate Team
            var team = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.TeamId);
            if (team == null)
            {
                return Results.BadRequest(new { error = $"Team with ID {request.TeamId} not found." });
            }

            // Create user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = "Player",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            // Create player profile
            var player = new Player
            {
                UserId = user.UserId,
                TeamId = request.TeamId,
                Position = request.Position ?? string.Empty,
                Age = request.Age,
                Height = request.Height ?? string.Empty,
                Weight = request.Weight,
                JerseyNumber = request.JerseyNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Players.Add(player);
            await db.SaveChangesAsync();

            // Auto sign-in
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Created($"/api/players/{player.PlayerId}", new
            {
                Message = "Player registered and signed in.",
                User = new { user.UserId, user.Email, user.FirstName, user.LastName, user.Role },
                Player = new { player.PlayerId, player.TeamId, player.Position, player.JerseyNumber }
            });
        });

        // POST /auth/register/coach
        group.MapPost("/register/coach", async (RegisterCoachRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, and Password are required." });
            }

            // Enforce unique email
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return Results.Conflict(new { error = "A user with this email already exists." });
            }

            // Create user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = "Coach",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            // Create coach profile (no team yet; can be assigned later or via team creation)
            var coach = new Coach
            {
                UserId = user.UserId,
                TeamId = null,
                ExperienceYears = request.ExperienceYears ?? 0,
                Specialty = request.Specialty ?? "General Coaching",
                Certification = request.Certification ?? "Basic",
                Bio = request.Bio ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Coaches.Add(coach);
            await db.SaveChangesAsync();

            // Auto sign-in
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Created($"/api/coaches/{coach.CoachId}", new
            {
                Message = "Coach registered and signed in.",
                User = new { user.UserId, user.Email, user.FirstName, user.LastName, user.Role },
                Coach = new { coach.CoachId, coach.ExperienceYears, coach.Specialty }
            });
        });
    }
}
