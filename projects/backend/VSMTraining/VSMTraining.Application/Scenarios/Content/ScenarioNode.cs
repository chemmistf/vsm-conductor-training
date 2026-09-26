using System.Text.Json.Serialization;

namespace VSMTraining.Application.Scenarios;

public class ScenarioNode
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;
    
    [JsonPropertyName("timer_seconds")]
    public int? TimerSeconds { get; set; }

    [JsonPropertyName("timeout_outcome")]
    public ScenarioTimeoutOutcome? TimeoutOutcome { get; set; }
    
    [JsonPropertyName("result_status")]
    public string? ResultStatus { get; set; }
    
    [JsonPropertyName("choices")]
    public List<ScenarioChoice>? Choices { get; set; }
}
