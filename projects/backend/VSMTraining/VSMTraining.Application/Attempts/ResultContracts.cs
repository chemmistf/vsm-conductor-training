namespace VSMTraining.Application.Attempts;

public record ScaleSummaryDto(int Initial, int Final);

public record CriticalErrorDto(string NodeId, string ChoiceId, string? Code, string ChoiceText);

public record CompetencyResultDto(string Code, string Name, int Score, string Level);

public record TimelineEntryDto(
    string NodeId,
    string? ChoiceId,
    string NodeText,
    string? ChoiceText,
    string? OutcomeText,
    int SafetyBefore,
    int SafetyDelta,
    int SafetyAfter,
    int LoyaltyBefore,
    int LoyaltyDelta,
    int LoyaltyAfter,
    bool CriticalError,
    string? CriticalErrorCode);

public record ResultResponse(
    Guid AttemptId,
    string ResultStatus,
    string ResultText,
    int EarnedXp,
    long TotalXp,
    int Level,
    ScaleSummaryDto Safety,
    ScaleSummaryDto Loyalty,
    List<CriticalErrorDto> CriticalErrors,
    List<CompetencyResultDto> Competencies,
    List<TimelineEntryDto> Timeline);
