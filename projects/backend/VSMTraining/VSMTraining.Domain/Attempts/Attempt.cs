using VSMTraining.Domain.Competencies;
using VSMTraining.Domain.Enums;
using VSMTraining.Domain.Scenarios;
using VSMTraining.Domain.Users;

namespace VSMTraining.Domain.Attempts;

public class Attempt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; } = null!;

    public Guid ScenarioId { get; set; }
    public Scenario Scenario { get; set; } = null!;

    public Guid ScenarioVersionId { get; set; }
    public ScenarioVersion ScenarioVersion { get; set; } = null!;

    public AttemptMode Mode { get; set; }
    public string? TargetServiceClass { get; set; }

    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }

    public string? CurrentNodeId { get; set; }

    public AttemptLifecycleStatus LifecycleStatus { get; set; } = AttemptLifecycleStatus.InProgress;
    
    /// <summary>Момент входа в текущий узел — точка отсчёта для таймера и response_time.</summary>
    public DateTimeOffset? CurrentNodeStartedAt { get; set; }
    
    /// <summary>Серверный дедлайн текущего узла. Null, если узел без таймера.</summary>
    public DateTimeOffset? NodeDeadlineAt { get; set; }

    // Шкалы состояния
    public int InitialSafety { get; set; }
    public int CurrentSafety { get; set; }
    public int? FinalSafety { get; set; }

    public int InitialLoyalty { get; set; }
    public int CurrentLoyalty { get; set; }
    public int? FinalLoyalty { get; set; }

    public AttemptResultStatus? ResultStatus { get; set; }
    public int CriticalErrorsCount { get; set; } = 0;

    // Фиксированный рандом (jsonb)
    public string? ActiveModifiersJson { get; set; }
    public string? SelectedVariantsJson { get; set; }
    public string? ContextJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<AttemptEvent> Events { get; set; } = new List<AttemptEvent>();
    public ICollection<AttemptMessage> Messages { get; set; } = new List<AttemptMessage>();
    public ICollection<AttemptCompetency> Competencies { get; set; } = new List<AttemptCompetency>();
    public Certification? Certification { get; set; }
}