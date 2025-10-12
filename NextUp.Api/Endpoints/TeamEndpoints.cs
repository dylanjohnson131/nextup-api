using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;

namespace NextUp.Api.Endpoints
{
    public static class TeamEndpoints
    {
        public static void MapTeamEndpoints(this WebApplication app)
        {
            var teamGroup = app.MapGroup("/api/teams");

            // GET /api/teams - Get all teams
            teamGroup.MapGet("/", async (NextUpDbContext db) =>
            {
                var teams = await db.Teams
                    .Include(team => team.Coach)
                        .ThenInclude(user => user.Coach)
                    .Include(team => team.Players)
                        .ThenInclude(player => player.User)
                    .ToListAsync();

                return Results.Ok(teams.Select(t => new
                {
                    t.TeamId,
                    t.Name,
                    t.Location,
                    t.IsPublic,
                    t.CreatedAt,
                    Coach = t.Coach != null ? new
                    {
                        CoachId = t.Coach.Coach?.CoachId,
                        Name = $"{t.Coach.FirstName} {t.Coach.LastName}",
                        ExperienceYears = t.Coach.Coach?.ExperienceYears ?? 0,
                        Specialty = t.Coach.Coach?.Specialty
                    } : null,
                    PlayerCount = t.Players?.Count ?? 0,
                    Players = (t.Players?.Select(p => (object)new
                    {
                        p.PlayerId,
                        Name = $"{p.User?.FirstName} {p.User?.LastName}",
                        p.Position,
                        p.JerseyNumber,
                        p.Age
                    })) ?? Array.Empty<object>()
                }));
            });

            // GET /api/teams/{id} - Get team by ID
            teamGroup.MapGet("/{id:int}", async (int id, NextUpDbContext db) =>
            {
                var team = await db.Teams
                    .Include(t => t.Coach)
                        .ThenInclude(u => u.Coach)
                    .Include(t => t.Players)
                        .ThenInclude(p => p.User)
                    .Include(t => t.HomeGames)
                    .Include(t => t.AwayGames)
                    .FirstOrDefaultAsync(t => t.TeamId == id);

                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });

                return Results.Ok(new
                {
                    team.TeamId,
                    team.Name,
                    team.Location,
                    team.IsPublic,
                    team.CreatedAt,
                    Coach = team.Coach != null ? new
                    {
                        CoachId = team.Coach.Coach?.CoachId,
                        Name = $"{team.Coach.FirstName} {team.Coach.LastName}",
                        Email = team.Coach.Email,
                        ExperienceYears = team.Coach.Coach?.ExperienceYears ?? 0,
                        Specialty = team.Coach.Coach?.Specialty,
                        Certification = team.Coach.Coach?.Certification,
                        Bio = team.Coach.Coach?.Bio
                    } : null,
                    Players = (team.Players?.Select(p => (object)new
                    {
                        p.PlayerId,
                        Name = $"{p.User?.FirstName} {p.User?.LastName}",
                        Email = p.User?.Email,
                        p.Position,
                        p.JerseyNumber,
                        p.Age,
                        p.Height,
                        p.Weight
                    })) ?? Array.Empty<object>(),
                    Stats = new
                    {
                        TotalPlayers = team.Players?.Count ?? 0,
                        HomeGames = team.HomeGames?.Count ?? 0,
                        AwayGames = team.AwayGames?.Count ?? 0
                    }
                });
            });

            // POST /api/teams - Create new team
            teamGroup.MapPost("/", async (CreateTeamRequest request, NextUpDbContext db, IPasswordService passwordService) =>
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Location))
                {
                    return Results.BadRequest(new { error = "Team name and location are required." });
                }

                // Check if coach user exists or create new one
                var coachUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.CoachEmail);

                if (coachUser == null)
                {
                    if (string.IsNullOrWhiteSpace(request.CoachFirstName) || string.IsNullOrWhiteSpace(request.CoachLastName))
                    {
                        return Results.BadRequest(new { error = "Coach first name and last name are required for new coaches." });
                    }

                    coachUser = new User
                    {
                        FirstName = request.CoachFirstName,
                        LastName = request.CoachLastName,
                        Email = request.CoachEmail,
                        PasswordHash = passwordService.HashPassword(request.CoachPassword ?? "defaultpassword123"),
                        Role = "Coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Users.Add(coachUser);
                    await db.SaveChangesAsync();
                }

                // Create the team
                var team = new Team
                {
                    Name = request.Name,
                    Location = request.Location,
                    CoachId = coachUser.UserId,
                    IsPublic = request.IsPublic,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Teams.Add(team);
                await db.SaveChangesAsync();

                // Create coach profile if doesn't exist
                var existingCoach = await db.Coaches.FirstOrDefaultAsync(c => c.UserId == coachUser.UserId);
                if (existingCoach == null)
                {
                    var coach = new Coach
                    {
                        UserId = coachUser.UserId,
                        TeamId = team.TeamId,
                        ExperienceYears = request.CoachExperience ?? 1,
                        Specialty = request.CoachSpecialty ?? "General Coaching",
                        Certification = request.CoachCertification ?? "Basic",
                        Bio = request.CoachBio ?? "Experienced football coach",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.Coaches.Add(coach);
                    await db.SaveChangesAsync();
                }

                return Results.Created($"/api/teams/{team.TeamId}", new 
                { 
                    team.TeamId, 
                    team.Name, 
                    team.Location, 
                    team.IsPublic,
                    CoachName = $"{coachUser.FirstName} {coachUser.LastName}",
                    CoachEmail = coachUser.Email
                });
            });

            // PUT /api/teams/{id} - Update team
            teamGroup.MapPut("/{id:int}", async (int id, UpdateTeamRequest request, NextUpDbContext db) =>
            {
                var team = await db.Teams.FindAsync(id);
                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(request.Name))
                    team.Name = request.Name;
                if (!string.IsNullOrWhiteSpace(request.Location))
                    team.Location = request.Location;
                if (request.IsPublic.HasValue)
                    team.IsPublic = request.IsPublic.Value;

                team.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();
                return Results.Ok(new 
                { 
                    team.TeamId, 
                    team.Name, 
                    team.Location, 
                    team.IsPublic,
                    UpdatedAt = team.UpdatedAt
                });
            });

            // DELETE /api/teams/{id} - Delete team
            teamGroup.MapDelete("/{id:int}", async (int id, NextUpDbContext db) =>
            {
                var team = await db.Teams
                    .Include(t => t.Players)
                    .Include(t => t.HomeGames)
                    .Include(t => t.AwayGames)
                    .FirstOrDefaultAsync(t => t.TeamId == id);

                if (team == null)
                    return Results.NotFound(new { error = $"Team with ID {id} not found." });

                // Check if team has active games
                if (team.HomeGames.Any() || team.AwayGames.Any())
                {
                    return Results.BadRequest(new 
                    { 
                        error = "Cannot delete team with existing games. Please remove all games first.",
                        homeGames = team.HomeGames.Count,
                        awayGames = team.AwayGames.Count
                    });
                }

                // Check if team has players
                if (team.Players.Any())
                {
                    return Results.BadRequest(new 
                    { 
                        error = "Cannot delete team with players. Please remove all players first.",
                        playerCount = team.Players.Count
                    });
                }

                db.Teams.Remove(team);
                await db.SaveChangesAsync();
                return Results.Ok(new { message = $"Team '{team.Name}' has been deleted successfully." });
            });
        }
    }
}