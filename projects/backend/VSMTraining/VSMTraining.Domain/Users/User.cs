using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Enums;

namespace VSMTraining.Domain.Users;

public class User
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public string? Depot { get; set; }
    public string? Brigade { get; set; }

    public int Level { get; set; } = 1;
    public long Xp { get; set; } = 0;
    
    public string? CurrentServiceClass { get; set; }
    public CertificationStatus CertificationStatus { get; set; } = CertificationStatus.None;
    
    public DateTimeOffset CreatedAt { get; set; } 
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}