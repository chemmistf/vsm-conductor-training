using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Attempts;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class AttemptConfiguration : IEntityTypeConfiguration<Attempt>
{
    public void Configure(EntityTypeBuilder<Attempt> builder)
    {
        builder.ToTable("attempts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Mode)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.TargetServiceClass).HasMaxLength(100);
        builder.Property(a => a.CurrentNodeId).HasMaxLength(200);
        
        builder.Property(a => a.LifecycleStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.ResultStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.CriticalErrorsCount).HasDefaultValue(0);

        builder.Property(a => a.ActiveModifiersJson).HasColumnType("jsonb");
        builder.Property(a => a.SelectedVariantsJson).HasColumnType("jsonb");
        builder.Property(a => a.ContextJson).HasColumnType("jsonb");

        builder.Property(a => a.StartedAt).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.ScenarioId);
        builder.HasIndex(a => a.LifecycleStatus);
        builder.HasIndex(a => a.ResultStatus);

        builder.HasMany(a => a.Events)
            .WithOne(e => e.Attempt)
            .HasForeignKey(e => e.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Messages)
            .WithOne(m => m.Attempt)
            .HasForeignKey(m => m.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Competencies)
            .WithOne(c => c.Attempt)
            .HasForeignKey(c => c.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}