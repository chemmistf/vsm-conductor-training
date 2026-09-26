using VSMTraining.Domain.Enums;
using VSMTraining.Domain.Users;

namespace VSMTraining.Domain.Competencies;

public class UserCompetency
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid CompetencyId { get; set; }
    public Competency Competency { get; set; } = null!;

    public int Score { get; set; }
    public CompetencyLever Level { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
