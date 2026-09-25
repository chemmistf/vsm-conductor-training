using VSMTraining.Domain.Attempts;

namespace VSMTraining.Domain.Scenarios;

public class ScenarioVersion
{
    public Guid Id { get; set; }
    
    public Guid ScenarioId { get; set; }
    public Scenario Scenario { get; set; } = null!;
    
    public int Version { get; set; }

    /// <summary>Полный граф сценария: узлы, варианты, правила, модификаторы. Хранится как jsonb.</summary>
    public string ContentJson { get; set; } = null!;
    
    public string? SourceDescription { get; set; }
    public string? CreateBy { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = false;
    
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}