namespace NextUp.Api.DTOs;
public record CreateGameRequest(
    int HomeTeamId,
    int AwayTeamId,
    DateTime GameDate,
    string Location,
    string Season
);
public record UpdateGameRequest(
    DateTime? GameDate,
    string? Location,
    int? HomeScore,
    int? AwayScore,
    string? Status
);
