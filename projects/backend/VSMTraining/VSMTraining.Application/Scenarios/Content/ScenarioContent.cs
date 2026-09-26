using System.Text.Json.Serialization;

namespace VSMTraining.Application.Scenarios;

public class ScenarioContent
{
    [JsonPropertyName("start_node")] 
    public string StartNode { get; set; } = null!;

    [JsonPropertyName("initial_state")]
    public ScenarioInitialState InitialState { get; set; } = null!;
    
    [JsonPropertyName("nodes")]
    public Dictionary<string, ScenarioNode> Nodes { get; set; } = new();
}