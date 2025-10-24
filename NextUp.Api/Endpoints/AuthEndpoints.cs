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
        group.MapPost("/register/player", async (RegisterPlayerRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, and Password are required." });
            }
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return Results.Conflict(new { error = "A user with this email already exists." });
            }
            var team = await db.Teams.FirstOrDefaultAsync(t => t.TeamId == request.TeamId);
            if (team == null)
            {
                return Results.BadRequest(new { error = $"Team with ID {request.TeamId} not found." });
            }
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = User.PLAYER_ROLE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
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
        group.MapPost("/register/coach", async (RegisterCoachRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, and Password are required." });
            }
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return Results.Conflict(new { error = "A user with this email already exists." });
            }
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = User.COACH_ROLE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
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
        group.MapPost("/register/athletic-director", async (RegisterAthleticDirectorRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { error = "FirstName, LastName, Email, and Password are required." });
            }
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return Results.Conflict(new { error = "A user with this email already exists." });
            }
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordService.HashPassword(request.Password),
                Role = User.ATHLETIC_DIRECTOR_ROLE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var athleticDirector = new AthleticDirector
            {
                UserId = user.UserId,
                Department = request.Department ?? string.Empty,
                ExperienceYears = request.ExperienceYears ?? 0,
                Certification = request.Certification ?? string.Empty,
                Institution = request.Institution ?? string.Empty,
                Bio = request.Bio ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.AthleticDirectors.Add(athleticDirector);
            await db.SaveChangesAsync();
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
            return Results.Created($"/api/athletic-directors/{athleticDirector.AthleticDirectorId}", new
            {
                Message = "Athletic Director registered and signed in.",
                User = new { user.UserId, user.Email, user.FirstName, user.LastName, user.Role },
                AthleticDirector = new { athleticDirector.AthleticDirectorId, athleticDirector.Department, athleticDirector.Institution }
            });
        });
    }
}
