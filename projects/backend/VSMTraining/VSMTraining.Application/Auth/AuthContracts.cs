namespace VSMTraining.Application.Auth;

public record RegisterRequest(string? Name, string? Email, string? Password, string? PasswordConfirmation);

public record LoginRequest(string? Email, string? Password, bool RememberMe);

public record UserDto(Guid Id, string Name, string Email);

public record AuthResponse(UserDto User, DateTimeOffset ExpiresAt);