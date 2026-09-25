using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Competencies;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class AttemptCompetencyConfiguration : IEntityTypeConfiguration<AttemptCompetency>
{
    public void Configure(EntityTypeBuilder<AttemptCompetency> builder)
    {
        builder.ToTable("attempt_competencies");

        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.PositiveSignals).HasDefaultValue(0);
        builder.Property(ac => ac.NegativeSignals).HasDefaultValue(0);

        builder.Property(ac => ac.Level)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ac => ac.Summary).HasColumnType("text");
        builder.Property(ac => ac.CreatedAt).IsRequired();

        // Одна компетенция один раз на попытку
        builder.HasIndex(ac => new { ac.AttemptId, ac.CompetencyId }).IsUnique();
    }
}