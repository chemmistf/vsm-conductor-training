using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSMTraining.Domain.Attempts;

namespace VSMTraining.Infrastructure.Persistence.Configurations;

public class AttemptMessageConfiguration : IEntityTypeConfiguration<AttemptMessage>
{
    public void Configure(EntityTypeBuilder<AttemptMessage> builder)
    {
        builder.ToTable("attempt_messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.NodeId).IsRequired().HasMaxLength(200);

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.Content).IsRequired().HasColumnType("text");

        builder.Property(m => m.Provider).HasMaxLength(100);
        builder.Property(m => m.Model).HasMaxLength(150);

        builder.Property(m => m.EvaluationJson).HasColumnType("jsonb");

        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasIndex(m => m.AttemptId);
    }
}