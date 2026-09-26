namespace VSMTraining.Application.Competencies;

public static class CompetencyCatalog
{
    private static readonly IReadOnlyDictionary<string, string> Names =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["situation_assessment"] = "Оценка ситуации",
            ["communication"] = "Коммуникация",
            ["prioritization"] = "Приоритизация",
            ["safety_compliance"] = "Соблюдение безопасности",
            ["time_management"] = "Управление временем"
        };

    public static string GetName(string code) =>
        Names.TryGetValue(code, out var name) ? name : code;
}
