namespace VSMTraining.Application.Attempts;

/// <summary>
/// Единственный источник идентификаторов демо-данных.
/// </summary>
public static class DemoDataIds
{
    public static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public const string ScenarioCode = "intoxicated_passenger";
    public const int ScenarioVersion = 1;
}