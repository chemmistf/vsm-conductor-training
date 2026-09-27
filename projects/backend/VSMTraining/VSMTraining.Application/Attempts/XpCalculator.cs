using VSMTraining.Application.Scenarios;

namespace VSMTraining.Application.Attempts;

public sealed record XpBreakdown(int CompletionXp, int ResultBonus, int CompetencyBonus)
{
    public int Total => CompletionXp + ResultBonus + CompetencyBonus;
}

public static class XpCalculator
{
    public const int CompletionXp = 40;
    public const int CompetencyXpPerPositivePoint = 5;
    public const int CompetencyBonusCap = 30;

    public static XpBreakdown Calculate(string resultStatus, IEnumerable<int> competencyScores)
    {
        var resultBonus = resultStatus switch
        {
            ScenarioResultStatuses.Success => 50,
            ScenarioResultStatuses.Failed => 20,
            ScenarioResultStatuses.CriticalFailure => 0,
            _ => 0
        };

        var positiveScore = competencyScores
            .Where(score => score > 0)
            .Sum();
        var competencyBonus = Math.Min(
            CompetencyBonusCap,
            positiveScore * CompetencyXpPerPositivePoint);

        return new XpBreakdown(CompletionXp, resultBonus, competencyBonus);
    }

    public static int CalculateLevel(long xp) => checked((int)(xp / 500) + 1);
}
