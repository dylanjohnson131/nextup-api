namespace NextUp.Api.DTOs;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role
);

public record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Password,
    string? Role
);
