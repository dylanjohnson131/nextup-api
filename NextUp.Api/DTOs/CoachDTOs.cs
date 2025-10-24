namespace NextUp.Api.DTOs;
public record CreateCoachRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Password,
    int? UserId,
    int? TeamId,
    int? ExperienceYears,
    string? Specialty,
    string? Certification,
    string? Bio
);
public record UpdateCoachRequest(
    int? TeamId,
    int? ExperienceYears,
    string? Specialty,
    string? Certification,
    string? Bio
);
