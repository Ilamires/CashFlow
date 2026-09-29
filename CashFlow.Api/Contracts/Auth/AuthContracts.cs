namespace CashFlow.Api.Contracts.Auth;

public record RegisterRequest(
    string Email,
    string Password,
    string? FirstName = null,
    string? LastName = null);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, DateTimeOffset ExpiresAt);

public record MeResponse(long Id, string Email, string? FirstName, string? LastName);