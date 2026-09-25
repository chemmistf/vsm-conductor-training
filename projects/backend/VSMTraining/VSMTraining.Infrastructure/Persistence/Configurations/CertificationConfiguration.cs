using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Users;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> builder)
    {
        builder.ToTable("certifications");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ServiceClass).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.IssuedAt).IsRequired();

        // Один Attempt -> максимум одна сертификация
        builder.HasIndex(c => c.AttemptId).IsUnique();

        builder.HasOne(c => c.Attempt)
            .WithOne(a => a.Certification)
            .HasForeignKey<Certification>(c => c.AttemptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}