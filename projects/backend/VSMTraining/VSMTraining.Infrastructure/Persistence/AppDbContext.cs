using Microsoft.EntityFrameworkCore;
using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Competencies;
using VSMTraining.Domain.Scenarios;
using VSMTraining.Domain.Users;

namespace VSMTraining.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Certification> Certifications => Set<Certification>();
    
    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<ScenarioVersion> ScenarioVersions => Set<ScenarioVersion>();
    
    public DbSet<Attempt> Attempts => Set<Attempt>();
    public DbSet<AttemptEvent> AttemptEvents => Set<AttemptEvent>();
    
    public DbSet<Competency> Competencies => Set<Competency>();
    public DbSet<AttemptCompetency> AttemptCompetencies => Set<AttemptCompetency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
