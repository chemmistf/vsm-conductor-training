using System.Text.Json;
using VSMTraining.Domain.Enums;

namespace VSMTraining.Application.Scenarios;

public record ChoiceResolutionResult(int SafetyAfter, int LoyaltyAfter, string NextNodeId);

/// <summary>
/// Чистая логика сценария: не знает про EF/БД. Разбор, валидация и применение правил графа.
/// </summary>
public static class ScenarioRuntime
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ScenarioContent Parse(string json)
    {
        var content = JsonSerializer.Deserialize<ScenarioContent>(json, SerializerOptions);
        if (content is null)
        {
            throw new InvalidOperationException("Scenario content is empty or invalid JSON.");
        }

        return content;
    }

    public static ScenarioNode GetNode(ScenarioContent content, string nodeId)
    {
        if (!content.Nodes.TryGetValue(nodeId, out var node))
        {
            throw new InvalidOperationException($"Node '{nodeId}' does not exist in scenario content.");
        }

        return node;
    }

    public static List<string> Validate(ScenarioContent content)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(content.StartNode))
        {
            errors.Add("start_node is required.");
        }
        else if (!content.Nodes.ContainsKey(content.StartNode))
        {
            errors.Add($"start_node '{content.StartNode}' does not exist among nodes.");
        }

        if (content.InitialState is null)
        {
            errors.Add("initial_state is required.");
        }
        else
        {
            if (content.InitialState.Safety is < 0 or > 100)
                errors.Add("initial_state.safety must be between 0 and 100.");
            if (content.InitialState.Loyalty is < 0 or > 100)
                errors.Add("initial_state.loyalty must be between 0 and 100.");
        }

        if (!content.Nodes.Values.Any(n => n.Type == ScenarioNodeTypes.Result))
        {
            errors.Add("scenario must contain at least one result node.");
        }

        foreach (var (nodeId, node) in content.Nodes)
        {
            switch (node.Type)
            {
                case ScenarioNodeTypes.Decision:
                    ValidateDecisionNode(nodeId, node, content, errors);
                    break;
                case ScenarioNodeTypes.Result:
                    ValidateResultNode(nodeId, node, errors);
                    break;
                default:
                    errors.Add($"node '{nodeId}' has unknown type '{node.Type}'.");
                    break;
            }
        }

        return errors;
    }

    private static void ValidateDecisionNode(string nodeId, ScenarioNode node, ScenarioContent content,
        List<string> errors)
    {
        var choices = node.Choices ?? new List<ScenarioChoice>();

        if (choices.Count is < 2 or > 4)
        {
            errors.Add($"node '{nodeId}' must have 2 to 4 choices, has {choices.Count}.");
        }

        var choiceIds = new HashSet<string>();
        foreach (var choice in choices)
        {
            if (!choiceIds.Add(choice.Id))
            {
                errors.Add($"node '{nodeId}' has duplicate choice id '{choice.Id}'.");
            }

            if (!content.Nodes.ContainsKey(choice.NextNode))
            {
                errors.Add($"node '{nodeId}' choice '{choice.Id}' has unknown next_node '{choice.NextNode}'.");
            }

            if (choice.ConditionalNext is not null && !content.Nodes.ContainsKey(choice.ConditionalNext.NextNode))
            {
                errors.Add(
                    $"node '{nodeId}' choice '{choice.Id}' has unknown conditional_next.next_node '{choice.ConditionalNext.NextNode}'.");
            }

            foreach (var signal in choice.Competencies.Values)
            {
                if (signal is < -1 or > 1)
                {
                    errors.Add(
                        $"node '{nodeId}' choice '{choice.Id}' has invalid competency signal '{signal}' (must be -1, 0 or 1).");
                }
            }
        }

        if (node.TimerSeconds.HasValue && string.IsNullOrWhiteSpace(node.TimeoutNextNode))
        {
            errors.Add($"node '{nodeId}' has timer_seconds but no timeout_next_node.");
        }

        if (!string.IsNullOrWhiteSpace(node.TimeoutNextNode) && !content.Nodes.ContainsKey(node.TimeoutNextNode))
        {
            errors.Add($"node '{nodeId}' has unknown timeout_next_node '{node.TimeoutNextNode}'.");
        }
    }

    private static void ValidateResultNode(string nodeId, ScenarioNode node, List<string> errors)
    {
        if (node.ResultStatus is not (ScenarioResultStatuses.Success
            or ScenarioResultStatuses.Failed
            or ScenarioResultStatuses.CriticalFailure))
        {
            errors.Add($"result node '{nodeId}' has invalid result_status '{node.ResultStatus}'.");
        }
    }

    public static ChoiceResolutionResult ResolveChoice(ScenarioChoice choice, int currentSafety, int currentLoyalty)
    {
        var safetyAfter = Clamp(currentSafety + choice.SafetyDelta);
        var loyaltyAfter = Clamp(currentLoyalty + choice.LoyaltyDelta);

        var nextNodeId = choice.NextNode;
        if (choice.ConditionalNext is not null &&
            EvaluateConditionalNext(choice.ConditionalNext, safetyAfter, loyaltyAfter))
        {
            nextNodeId = choice.ConditionalNext.NextNode;
        }

        return new ChoiceResolutionResult(safetyAfter, loyaltyAfter, nextNodeId);
    }

    private static bool EvaluateConditionalNext(ConditionalNext condition, int safety, int loyalty)
    {
        var metricValue = condition.Metric switch
        {
            "safety" => safety,
            "loyalty" => loyalty,
            _ => throw new InvalidOperationException($"Unknown conditional_next metric '{condition.Metric}'.")
        };

        return condition.Operator switch
        {
            "lt" => metricValue < condition.Value,
            "lte" => metricValue <= condition.Value,
            "gt" => metricValue > condition.Value,
            "gte" => metricValue >= condition.Value,
            "eq" => metricValue == condition.Value,
            _ => throw new InvalidOperationException($"Unknown conditional_next operator '{condition.Operator}'.")
        };
    }

    private static int Clamp(int value) => Math.Clamp(value, 0, 100);

    public static AttemptResultStatus ParseResultStatus(string value) => value switch
    {
        ScenarioResultStatuses.Success => AttemptResultStatus.Success,
        ScenarioResultStatuses.Failed => AttemptResultStatus.Failed,
        ScenarioResultStatuses.CriticalFailure => AttemptResultStatus.CriticalFailure,
        _ => throw new InvalidOperationException($"Unknown result_status '{value}'.")
    };

    /// <summary>
    /// Суммирует сигналы компетенций из EventDataJson всех событий попытки.
    /// Компетенция, ни разу не встретившаяся в событиях, в результат не попадает.
    /// </summary>
    public static Dictionary<string, int> AggregateCompetencies(IEnumerable<string?> eventDataJsonEntries)
    {
        var totals = new Dictionary<string, int>();

        foreach (var json in eventDataJsonEntries)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                continue;
            }

            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("competencies", out var competenciesElement))
            {
                continue;
            }

            foreach (var signal in competenciesElement.EnumerateObject())
            {
                totals[signal.Name] = totals.GetValueOrDefault(signal.Name) + signal.Value.GetInt32();
            }
        }

        return totals;
    }

    public static string CompetencyLevel(int score) => score switch
    {
        >= 2 => "strength",
        <= -1 => "development_area",
        _ => "stable"
    };
}