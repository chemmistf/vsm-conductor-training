namespace VSMTraining.Application.Leaderboard;

public record LeaderboardEntryDto(
    Guid UserId,
    int Rank,
    string Name,
    string ServiceClass,
    long Xp,
    int Level,
    int LevelProgressPercent,
    bool IsCurrentUser);

public record LeaderboardResponse(
    string Period,
    string Scope,
    List<LeaderboardEntryDto> Entries);
