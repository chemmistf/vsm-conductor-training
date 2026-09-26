namespace VSMTraining.Application.Attempts;

public record ScaleSummaryDto(int Initial, int Final);

public record CriticalErrorDto(string NodeId, string ChoiceId, string? Code, string ChoiceText);

public record CompetencyResultDto(string Code, int Score, string Level);

public record TimelineEntryDto(string NodeId, string? ChoiceId, int SafetyDelta, int LoyaltyDelta, bool CriticalError);

public record ResultResponse(
    Guid AttemptId,
    string ResultStatus,
    string ResultText,
    ScaleSummaryDto Safety,
    ScaleSummaryDto Loyalty,
    List<CriticalErrorDto> CriticalErrors,
    List<CompetencyResultDto> Competencies,
    List<TimelineEntryDto> Timeline);