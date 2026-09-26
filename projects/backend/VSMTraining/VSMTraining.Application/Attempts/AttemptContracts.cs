namespace VSMTraining.Application.Attempts;

public record StartAttemptRequest(Guid? ScenarioId);

public record ChooseRequest(string ChoiceId);

public record AttemptChoiceDto(string Id, string Text);

public record AttemptNodeDto(
    string Id,
    string Type,
    string Text,
    List<AttemptChoiceDto> Choices,
    DateTimeOffset? DeadlineAt);

/// <summary>Единый response для start, choice и timeout.</summary>
public record AttemptStateResponse(
    Guid AttemptId,
    string Status,
    string? ResultStatus,
    int Safety,
    int Loyalty,
    bool Finished,
    AttemptNodeDto? Node);

public record ApiErrorResponse(string Code, string Message);