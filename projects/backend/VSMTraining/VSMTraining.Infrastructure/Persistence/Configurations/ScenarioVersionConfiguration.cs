using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Scenarios;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class ScenarioVersionConfiguration : IEntityTypeConfiguration<ScenarioVersion>
{
    public void Configure(EntityTypeBuilder<ScenarioVersion> builder)
    {
        builder.ToTable("scenario_versions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Version).IsRequired();

        builder.Property(v => v.ContentJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(v => v.SourceDescription).HasColumnType("text");
        builder.Property(v => v.CreatedBy).HasMaxLength(200);

        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.IsPublished).HasDefaultValue(false);

        // Один сценарий не может иметь два одинаковых номера версии
        builder.HasIndex(v => new { v.ScenarioId, v.Version }).IsUnique();

        builder.HasMany(v => v.Attempts)
            .WithOne(a => a.ScenarioVersion)
            .HasForeignKey(a => a.ScenarioVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}