using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Attempts;

public class AttemptMessage
{
    public Guid Id { get; set; } 
    
    public Guid AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public string NodeId { get; set; } = null!;
    public MessageRole Role { get; set; }
    public string Content { get; set; } = null!;
    
    public string? Provider { get; set; }
    public string? Model { get; set; }
    
    /// <summary>Структурированная оценка реплики от LLM (jsonb).</summary>
    public string? EvaluationJson { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}