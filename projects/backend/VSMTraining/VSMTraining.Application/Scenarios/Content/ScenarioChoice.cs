using System.Text.Json.Serialization;

namespace VSMTraining.Application.Scenarios;

public class ScenarioChoice
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;

    [JsonPropertyName("safety_delta")]
    public int SafetyDelta { get; set; }

    [JsonPropertyName("loyalty_delta")]
    public int LoyaltyDelta { get; set; }

    [JsonPropertyName("competencies")]
    public Dictionary<string, int> Competencies { get; set; } = new();

    [JsonPropertyName("critical_error")]
    public bool CriticalError { get; set; }

    [JsonPropertyName("critical_error_code")]
    public string? CriticalErrorCode { get; set; }

    [JsonPropertyName("next_node")]
    public string NextNode { get; set; } = null!;

    [JsonPropertyName("conditional_next")]
    public ConditionalNext? ConditionalNext { get; set; }
}