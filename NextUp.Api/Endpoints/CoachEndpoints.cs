using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
using System.Security.Claims;
using System.Security.Claims;

namespace NextUp.Api.Endpoints;

public static class CoachEndpoints
{
    public static void MapCoachEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/coaches");

        // GET /api/coaches/me
        group.MapGet("/me", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var coach = await db.Coaches
                .Include(c => c.User)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (coach == null)
                return Results.NotFound(new { error = "Coach profile not found." });

            return Results.Ok(new
            {
                coach.CoachId,
                Name = coach.User != null ? $"{coach.User.FirstName} {coach.User.LastName}" : null,
                Email = coach.User?.Email,
                coach.ExperienceYears,
                coach.Specialty,
                coach.Certification,
                coach.Bio,
                Team = coach.Team != null ? new { coach.Team.TeamId, coach.Team.Name, coach.Team.Location } : null
            });
        }).RequireAuthorization();

        // GET /api/coaches
        group.MapGet("/", async (NextUpDbContext db) =>
        {
            var coaches = await db.Coaches
                .Include(c => c.User)
                .Include(c => c.Team)
                .ToListAsync();

            return Results.Ok(coaches.Select(c => new
            {
                c.CoachId,
                Name = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : null,
                Email = c.User?.Email,
                c.ExperienceYears,
                c.Specialty,
                c.Certification,
                c.Bio,
                Team = c.Team != null ? new { c.Team.TeamId, c.Team.Name, c.Team.Location } : null
            }));
        });

        // GET /api/coaches/me
        group.MapGet("/me", async (NextUpDbContext db, System.Security.Claims.ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }

            var userId = int.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var coach = await db.Coaches
                .Include(c => c.User)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (coach == null)
                return Results.NotFound(new { error = "Coach profile not found." });

            return Results.Ok(new
            {
                coach.CoachId,
                Name = coach.User != null ? $"{coach.User.FirstName} {coach.User.LastName}" : null,
                Email = coach.User?.Email,
                coach.ExperienceYears,
                coach.Specialty,
                coach.Certification,
                coach.Bio,
                Team = coach.Team != null ? new { coach.Team.TeamId, coach.Team.Name, coach.Team.Location } : null
            });
        }).RequireAuthorization();

        // GET /api/coaches/{id}
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var coach = await db.Coaches
                .Include(c => c.User)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.CoachId == id);

            if (coach == null)
                return Results.NotFound(new { error = $"Coach with ID {id} not found." });

            return Results.Ok(new
            {
                coach.CoachId,
                Name = coach.User != null ? $"{coach.User.FirstName} {coach.User.LastName}" : null,
                Email = coach.User?.Email,
                coach.ExperienceYears,
                coach.Specialty,
                coach.Certification,
                coach.Bio,
                Team = coach.Team != null ? new { coach.Team.TeamId, coach.Team.Name, coach.Team.Location } : null
            });
        });

        // POST /api/coaches
        group.MapPost("/", async (CreateCoachRequest request, NextUpDbContext db, IPasswordService passwordService) =>
        {
            // Determine or create the User
            User? user = null;
            if (request.UserId.HasValue)
            {
                user = await db.Users.FindAsync(request.UserId.Value);
                if (user == null)
                    return Results.BadRequest(new { error = $"User with ID {request.UserId.Value} not found." });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email))
                {
                    return Results.BadRequest(new { error = "FirstName, LastName, and Email are required to create a new user." });
                }

                var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existing != null)
                {
                    return Results.Conflict(new { error = "A user with this email already exists." });
                }

                user = new User
                {
                    FirstName = request.FirstName!,
                    LastName = request.LastName!,
                    Email = request.Email!,
                    PasswordHash = passwordService.HashPassword(string.IsNullOrWhiteSpace(request.Password) ? Guid.NewGuid().ToString("N") : request.Password!),
                    Role = "Coach",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            // Optionally validate team
            Team? team = null;
            if (request.TeamId.HasValue)
            {
                team = await db.Teams.FindAsync(request.TeamId.Value);
                if (team == null)
                    return Results.BadRequest(new { error = $"Team with ID {request.TeamId.Value} not found." });
            }

            var coach = new Coach
            {
                UserId = user!.UserId,
                TeamId = request.TeamId,
                ExperienceYears = request.ExperienceYears ?? 0,
                Specialty = request.Specialty ?? "General Coaching",
                Certification = request.Certification ?? "Basic",
                Bio = request.Bio ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Coaches.Add(coach);
            await db.SaveChangesAsync();

            return Results.Created($"/api/coaches/{coach.CoachId}", new
            {
                coach.CoachId,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                coach.ExperienceYears,
                coach.Specialty,
                coach.Certification,
                coach.Bio,
                Team = team != null ? new { team.TeamId, team.Name, team.Location } : null
            });
        });

        // PUT /api/coaches/{id}
        group.MapPut("/{id:int}", async (int id, UpdateCoachRequest request, NextUpDbContext db) =>
        {
            var coach = await db.Coaches.FindAsync(id);
            if (coach == null)
                return Results.NotFound(new { error = $"Coach with ID {id} not found." });

            if (request.TeamId.HasValue)
            {
                var team = await db.Teams.FindAsync(request.TeamId.Value);
                if (team == null)
                    return Results.BadRequest(new { error = $"Team with ID {request.TeamId.Value} not found." });
                coach.TeamId = request.TeamId.Value;
            }
            if (request.ExperienceYears.HasValue) coach.ExperienceYears = request.ExperienceYears.Value;
            if (request.Specialty != null) coach.Specialty = request.Specialty;
            if (request.Certification != null) coach.Certification = request.Certification;
            if (request.Bio != null) coach.Bio = request.Bio;

            coach.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Coach updated", coach.CoachId, coach.TeamId, coach.ExperienceYears, coach.Specialty, coach.Certification, coach.Bio, coach.UpdatedAt });
        });

        // GET /api/coaches/me
        group.MapGet("/me", async (NextUpDbContext db, ClaimsPrincipal user) =>
        {
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }

            var userId = int.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var coach = await db.Coaches
                .Include(c => c.User)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (coach == null)
                return Results.NotFound(new { error = "Coach profile not found for current user." });

            return Results.Ok(new
            {
                coach.CoachId,
                Name = coach.User != null ? $"{coach.User.FirstName} {coach.User.LastName}" : null,
                Email = coach.User?.Email,
                coach.ExperienceYears,
                coach.Specialty,
                coach.Certification,
                coach.Bio,
                Team = coach.Team != null ? new { coach.Team.TeamId, coach.Team.Name, coach.Team.Location } : null
            });
        }).RequireAuthorization();

        // GET /api/coaches/{id}
        group.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
        {
            var coach = await db.Coaches
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.CoachId == id);
            if (coach == null)
                return Results.NotFound(new { error = $"Coach with ID {id} not found." });

            // If the coach is assigned to a team, block deletion to avoid orphaning the team
            if (coach.TeamId.HasValue)
            {
                return Results.BadRequest(new { error = "Cannot delete a coach assigned to a team. Reassign the team or remove coach from team first.", teamId = coach.TeamId });
            }

            db.Coaches.Remove(coach);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Coach {id} deleted." });
        });
    }
}
