using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Scenarios;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("scenarios");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code).IsRequired().HasMaxLength(150);
        builder.HasIndex(s => s.Code).IsUnique();

        builder.Property(s => s.Title).IsRequired().HasMaxLength(300);
        builder.Property(s => s.Category).HasMaxLength(100);

        builder.Property(s => s.Difficulty)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.IsActive).HasDefaultValue(true);

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasMany(s => s.Versions)
            .WithOne(v => v.Scenario)
            .HasForeignKey(v => v.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
