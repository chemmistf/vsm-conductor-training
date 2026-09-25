namespace VSMTraining.Domain.Comprtencies;

public class Competency
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<AttemptCompetency> AttemptCompetencies { get; set; } = new List<AttemptCompetency>();
}