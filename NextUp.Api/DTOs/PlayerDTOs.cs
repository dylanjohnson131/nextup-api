namespace NextUp.Api.DTOs;

public record CreatePlayerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Password,
    int TeamId,
    string Position,
    int? Age,
    string? Height,
    int? Weight,
    int? JerseyNumber
);

public record UpdatePlayerRequest(
    string? FirstName,
    string? LastName,
    string? Position,
    int? Age,
    string? Height,
    int? Weight,
    int? JerseyNumber
);
