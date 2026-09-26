using System.Text.Json.Serialization;

namespace VSMTraining.Application.Scenarios;

public class ConditionalNext
{
    [JsonPropertyName("metric")]
    public string Metric { get; set; } = null!;
    
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = null!;

    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("next_node")]
    public string NextNode { get; set; } = null!;
}