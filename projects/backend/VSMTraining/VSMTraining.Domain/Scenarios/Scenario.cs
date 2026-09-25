using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Scenarios;

public class Scenario
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public ScenarioDifficulty Difficulty { get; set; }

    public bool IsActive { get; set; } = true;
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<ScenarioVersion> Versions { get; set; } = new List<ScenarioVersion>();
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}