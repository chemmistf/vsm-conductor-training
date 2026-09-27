namespace VSMTraining.Application.Scenarios;

public record ScenarioSummaryDto(
    Guid Id,
    string Code,
    string Title,
    string? Description,
    string? Category,
    string Difficulty);

