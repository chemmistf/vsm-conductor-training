using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Attempts;

public class AttemptEvent
{
    public long Id { get; set; }
    
    public Guid AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public string NodeId { get; set; } = null!;
    public string? NodeVariantId { get; set; }
    public string? ChoiceId { get; set; }
    
    public AttemptEventType EventType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public int? ResponseTimeMs { get; set; }
    
    public int? SafetyBefore { get; set; }
    public int? SafetyDelta { get; set; }
    public int? SafetyAfter { get; set; }
    
    public int? LoyaltyBefore { get; set; }
    public int? LoyaltyDelta { get; set; }
    public int? LoyaltyAfter { get; set; }

    public bool CriticalError { get; set; } = false;
    public string? CriticalErrorCode { get; set; }
    
    /// <summary>Доп. контекст шага (jsonb).</summary>
    public string? EventDataJson { get; set; }
}