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

    /// <summary>
    /// Ordered transition rules. The first matching rule wins.
    /// Kept separate from conditional_next for backward compatibility with
    /// existing scenario JSON files that have a single condition.
    /// </summary>
    [JsonPropertyName("conditional_next_rules")]
    public List<ConditionalNext>? ConditionalNextRules { get; set; }
}
