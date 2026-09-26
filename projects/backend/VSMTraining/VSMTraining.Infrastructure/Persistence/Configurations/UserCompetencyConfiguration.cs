using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Competencies;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class UserCompetencyConfiguration : IEntityTypeConfiguration<UserCompetency>
{
    public void Configure(EntityTypeBuilder<UserCompetency> builder)
    {
        builder.ToTable("user_competencies");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.Level)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(uc => uc.CreatedAt).IsRequired();
        builder.Property(uc => uc.UpdatedAt).IsRequired();

        builder.HasIndex(uc => new { uc.UserId, uc.CompetencyId }).IsUnique();

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Competencies)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Competency)
            .WithMany(c => c.UserCompetencies)
            .HasForeignKey(uc => uc.CompetencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
