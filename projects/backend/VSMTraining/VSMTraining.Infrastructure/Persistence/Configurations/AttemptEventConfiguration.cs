using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Attempts;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class AttemptEventConfiguration : IEntityTypeConfiguration<AttemptEvent>
{
    public void Configure(EntityTypeBuilder<AttemptEvent> builder)
    {
        builder.ToTable("attempt_events");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.NodeId).IsRequired().HasMaxLength(200);
        builder.Property(e => e.NodeVariantId).HasMaxLength(200);
        builder.Property(e => e.ChoiceId).HasMaxLength(200);

        builder.Property(e => e.EventType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.CriticalErrorCode).HasMaxLength(150);
        builder.Property(e => e.CriticalError).HasDefaultValue(false);

        builder.Property(e => e.EventDataJson).HasColumnType("jsonb");

        builder.Property(e => e.OccurredAt).IsRequired();

        builder.HasIndex(e => e.AttemptId);
        builder.HasIndex(e => new { e.AttemptId, e.OccurredAt });
    }
}