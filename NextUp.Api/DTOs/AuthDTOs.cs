namespace NextUp.Api.DTOs;
public record LoginRequest(string Email, string Password);
public record RegisterPlayerRequest(
	string FirstName,
	string LastName,
	string Email,
	string Password,
	int TeamId,
	string? Position,
	int? Age,
	string? Height,
	int? Weight,
	int? JerseyNumber
);
public record RegisterCoachRequest(
	string FirstName,
	string LastName,
	string Email,
	string Password,
	int? ExperienceYears,
	string? Specialty,
	string? Certification,
	string? Bio
);
