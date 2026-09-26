namespace VSMTraining.Application.Competencies;

public record UserCompetencyDto(
    string Code,
    string Name,
    int Score,
    string Level);

public record UserCompetenciesResponse(Guid UserId, List<UserCompetencyDto> Competencies);
