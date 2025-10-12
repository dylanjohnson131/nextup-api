namespace NextUp.Api.DTOs;

public record CreateTeamRequest(
    string Name,
    string Location,
    bool IsPublic,
    string CoachEmail,
    string CoachFirstName,
    string CoachLastName,
    string? CoachPassword,
    int? CoachExperience,
    string? CoachSpecialty,
    string? CoachCertification,
    string? CoachBio
);

public record UpdateTeamRequest(
    string? Name,
    string? Location,
    bool? IsPublic
);
