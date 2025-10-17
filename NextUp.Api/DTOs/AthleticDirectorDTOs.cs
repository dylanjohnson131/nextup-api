namespace NextUp.Api.DTOs;

// Registration DTO for Athletic Director
public record RegisterAthleticDirectorRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Department,
    int? ExperienceYears,
    string? Certification,
    string? Institution,
    string? Bio
);

// Update Athletic Director Profile
public record UpdateAthleticDirectorRequest(
    string? Department,
    int? ExperienceYears,
    string? Certification,
    string? Institution,
    string? Bio
);

// Team Management DTOs for Athletic Director
public record CreateTeamByAdRequest(
    string Name,
    string? School,
    string? Mascot,
    string Location,
    string? City,
    string? State,
    string? Division,
    string? Conference,
    int? CoachId,
    bool IsPublic = true
);

public record UpdateTeamByAdRequest(
    string? Name,
    string? School,
    string? Mascot,
    string? Location,
    string? City,
    string? State,
    string? Division,
    string? Conference,
    bool? IsPublic,
    int? CoachId
);

// Game Management DTOs for Athletic Director
public record CreateGameByAdRequest(
    int HomeTeamId,
    int AwayTeamId,
    DateTime GameDate,
    string Location,
    string Season
);

public record UpdateGameByAdRequest(
    int? HomeTeamId,
    int? AwayTeamId,
    DateTime? GameDate,
    string? Location,
    string? Status
);

// Overview DTOs for Athletic Director Dashboard
public record SeasonOverviewResponse(
    int TotalTeams,
    int TotalGames,
    int CompletedGames,
    int UpcomingGames,
    IEnumerable<TeamSummary> Teams,
    IEnumerable<GameSummary> RecentGames
);

public record TeamSummary(
    int TeamId,
    string Name,
    string Location,
    string? CoachName,
    int PlayerCount,
    int GamesPlayed,
    int Wins,
    int Losses
);

public record GameSummary(
    int GameId,
    string HomeTeam,
    string AwayTeam,
    DateTime GameDate,
    string Location,
    bool IsCompleted,
    string? Score
);