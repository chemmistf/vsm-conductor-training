using System.Text.Json.Serialization;

namespace VSMTraining.Application.Scenarios;

public class ScenarioInitialState
{
    [JsonPropertyName("safety")]
    public int Safety { get; set; }
    
    [JsonPropertyName("loyalty")]
    public int Loyalty { get; set; }

}