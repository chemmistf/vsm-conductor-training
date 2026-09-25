using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Competencies;

public class AttemptCompetency
{
    public Guid Id { get; set; }
    
    public Guid AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;
    
    public Guid CompetencyId { get; set; }
    public Competency Competency { get; set; } = null!;

    public int PositiveSignals { get; set; } = 0;
    public int NegativeSignals { get; set; } = 0;
    public int Score { get; set; }
    public CompetencyLever Lever { get; set; }
    
    public string? Summary { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}