using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Competencies;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class CompetencyConfiguration : IEntityTypeConfiguration<Competency>
{
    public void Configure(EntityTypeBuilder<Competency> builder)
    {
        builder.ToTable("competencies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(150);
        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(300);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasMany(c => c.AttemptCompetencies)
            .WithOne(ac => ac.Competency)
            .HasForeignKey(ac => ac.CompetencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}