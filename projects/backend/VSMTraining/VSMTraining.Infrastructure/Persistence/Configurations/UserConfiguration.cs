using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Users;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ExternalId).IsRequired().HasMaxLength(200);
        builder.HasIndex(u => u.ExternalId).IsUnique();

        builder.Property(u => u.Name).IsRequired().HasMaxLength(300);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.ResetTokenHash).HasMaxLength(500);
        builder.Property(u => u.ResetTokenExpiresAt);
        builder.Property(u => u.ResetTokenUsedAt);

        builder.Property(u => u.Depot).HasMaxLength(200);
        builder.Property(u => u.Brigade).HasMaxLength(100);

        builder.Property(u => u.CurrentServiceClass).HasMaxLength(100);

        builder.Property(u => u.CertificationStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder.HasMany(u => u.Certifications)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Attempts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}