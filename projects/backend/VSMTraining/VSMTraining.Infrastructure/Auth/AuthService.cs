using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VSMTraining.Application.Auth;
using VSMTraining.Domain.Enums;
using VSMTraining.Domain.Users;
using VSMTraining.Infrastructure.Persistence;

namespace VSMTraining.Infrastructure.Auth;

public class AuthService
{
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(20);

    private readonly AppDbContext _db;
    private readonly PasswordHasherService _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IEmailSender _emailSender;
    private readonly PasswordResetOptions _passwordResetOptions;

    public AuthService(
        AppDbContext db,
        PasswordHasherService passwordHasher,
        JwtTokenService jwtTokenService,
        IEmailSender emailSender,
        IOptions<PasswordResetOptions> passwordResetOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _emailSender = emailSender;
        _passwordResetOptions = passwordResetOptions.Value;
    }

    public async Task<(UserDto User, string Token, DateTimeOffset ExpiresAt)> RegisterAsync(RegisterRequest request)
    {
        var name = (request.Name ?? string.Empty).Trim();
        var email = NormalizeEmail(request.Email);
        var password = request.Password ?? string.Empty;
        var passwordConfirmation = request.PasswordConfirmation ?? string.Empty;

        var errors = new List<string>();
        if (name.Length is < 2 or > 100)
            errors.Add("Имя должно содержать от 2 до 100 символов.");
        if (!IsValidEmail(email))
            errors.Add("Некорректный email.");
        if (password.Length < 8)
            errors.Add("Пароль должен содержать минимум 8 символов.");
        if (password != passwordConfirmation)
            errors.Add("Пароли не совпадают.");

        if (errors.Count > 0)
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, string.Join(" ", errors));

        var emailTaken = await _db.Users.AnyAsync(u => u.Email == email);
        if (emailTaken)
            throw new AuthException("email_already_exists", AuthStatusCodes.Conflict,
                "Пользователь с таким email уже зарегистрирован.");

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Level = 1,
            Xp = 0,
            CertificationStatus = CertificationStatus.None,
            CreatedAt = now,
            UpdatedAt = now
        };
        user.ExternalId = $"local-{user.Id:N}";
        user.PasswordHash = _passwordHasher.Hash(user, password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, expiresAt) = _jwtTokenService.GenerateToken(user.Id, rememberMe: false);
        return (ToDto(user), token, expiresAt);
    }

    public async Task<(UserDto User, string Token, DateTimeOffset ExpiresAt)> LoginAsync(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var password = request.Password ?? string.Empty;

        if (email.Length == 0)
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, "Email обязателен.");
        if (!IsValidEmail(email))
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, "Некорректный email.");
        if (password.Length == 0)
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, "Пароль обязателен.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if (user is null || !_passwordHasher.Verify(user, password))
            throw new AuthException("invalid_credentials", AuthStatusCodes.Unauthorized,
                "Неверный email или пароль.");

        var (token, expiresAt) = _jwtTokenService.GenerateToken(user.Id, request.RememberMe);
        return (ToDto(user), token, expiresAt);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        return user is null ? null : ToDto(user);
    }

    public async Task<string?> RequestPasswordResetAsync(string? email)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (normalizedEmail.Length == 0)
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, "Email обязателен.");
        if (!IsValidEmail(normalizedEmail))
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, "Некорректный email.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user is null)
            return null;

        var rawToken = GenerateResetToken();
        var now = DateTimeOffset.UtcNow;

        user.ResetTokenHash = HashToken(rawToken);
        user.ResetTokenExpiresAt = now.Add(ResetTokenLifetime);
        user.ResetTokenUsedAt = null;
        user.UpdatedAt = now;
        await _db.SaveChangesAsync();

        var resetUrl = BuildResetUrl(rawToken);
        await _emailSender.SendPasswordResetEmailAsync(user.Email, resetUrl);

        return resetUrl;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var token = request.Token ?? string.Empty;
        var password = request.Password ?? string.Empty;
        var passwordConfirmation = request.PasswordConfirmation ?? string.Empty;

        if (token.Length == 0)
            throw new AuthException("invalid_or_expired_reset_token", AuthStatusCodes.BadRequest,
                "Ссылка для восстановления недействительна.");

        var errors = new List<string>();
        if (password.Length < 8) errors.Add("Пароль должен содержать минимум 8 символов.");
        if (password != passwordConfirmation) errors.Add("Пароли не совпадают.");
        if (errors.Count > 0)
            throw new AuthException("validation_error", AuthStatusCodes.UnprocessableEntity, string.Join(" ", errors));

        var tokenHash = HashToken(token);
        var now = DateTimeOffset.UtcNow;

        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.ResetTokenHash == tokenHash &&
            u.ResetTokenUsedAt == null &&
            u.ResetTokenExpiresAt != null &&
            u.ResetTokenExpiresAt > now);

        if (user is null)
            throw new AuthException("invalid_or_expired_reset_token", AuthStatusCodes.BadRequest,
                "Ссылка для восстановления недействительна или уже использована.");

        user.PasswordHash = _passwordHasher.Hash(user, password);
        user.ResetTokenUsedAt = now;
        user.UpdatedAt = now;
        await _db.SaveChangesAsync();
    }

    private string BuildResetUrl(string rawToken)
    {
        var baseUrl = _passwordResetOptions.FrontendBaseUrl.TrimEnd('/');
        return $"{baseUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";
    }

    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.CultureInvariant);
    }

    private static UserDto ToDto(User user) => new(user.Id, user.Name, user.Email);
}

internal static class AuthStatusCodes
{
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Conflict = 409;
    public const int UnprocessableEntity = 422;
}
