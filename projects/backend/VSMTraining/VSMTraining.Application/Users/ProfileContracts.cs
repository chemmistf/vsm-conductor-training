namespace VSMTraining.Application.Users;

public record ProfileAchievementDto(
    string Id,
    string Title,
    string Subtitle,
    DateTimeOffset? EarnedAt);

public record ProfileResponse(
    Guid UserId,
    string Name,
    string Email,
    string? ServiceClass,
    int Level,
    long Xp,
    long XpInLevel,
    long XpToNextLevel,
    int LevelProgressPercent,
    string CertificationStatus,
    int CompletedAttempts,
    List<ProfileAchievementDto> Achievements);

