using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Users;

public class Certification
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string ServiceClass { get; set; } = null!;
    public CertificationResult Status { get; set; }
    
    public Guid AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;
    
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
