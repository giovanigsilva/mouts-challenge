using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class OutboxMessageAttemptConfiguration : IEntityTypeConfiguration<OutboxMessageAttempt>
{
    public void Configure(EntityTypeBuilder<OutboxMessageAttempt> builder)
    {
        builder.ToTable("OutboxMessageAttempts");

        builder.HasKey(attempt => attempt.Id);
        builder.Property(attempt => attempt.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(attempt => attempt.OutboxMessageId).HasColumnType("uuid").IsRequired();
        builder.Property(attempt => attempt.AttemptNumber).IsRequired();
        builder.Property(attempt => attempt.Status).IsRequired().HasMaxLength(50);
        builder.Property(attempt => attempt.Error);
        builder.Property(attempt => attempt.StartedAt).IsRequired();
        builder.Property(attempt => attempt.FinishedAt);
        builder.Property(attempt => attempt.DurationMs);
        builder.Property(attempt => attempt.WorkerInstanceId).HasMaxLength(200);
        builder.Property(attempt => attempt.CorrelationId).HasColumnType("uuid");

        builder.HasIndex(attempt => attempt.OutboxMessageId);
    }
}
