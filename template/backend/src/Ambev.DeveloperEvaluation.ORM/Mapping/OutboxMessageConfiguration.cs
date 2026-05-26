using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(message => message.EventId).HasColumnType("uuid").IsRequired();
        builder.Property(message => message.EventType).IsRequired().HasMaxLength(200);
        builder.Property(message => message.AggregateId).HasColumnType("uuid").IsRequired();
        builder.Property(message => message.AggregateType).IsRequired().HasMaxLength(100);
        builder.Property(message => message.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.Headers).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.CorrelationId).HasColumnType("uuid");
        builder.Property(message => message.CausationId).HasColumnType("uuid");
        builder.Property(message => message.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(message => message.RetryCount).IsRequired();
        builder.Property(message => message.MaxRetries).IsRequired();
        builder.Property(message => message.NextRetryAt);
        builder.Property(message => message.CreatedAt).IsRequired();
        builder.Property(message => message.PublishedAt);
        builder.Property(message => message.LastError);
        builder.Property(message => message.LockedUntil);
        builder.Property(message => message.LockId).HasColumnType("uuid");
        builder.Property(message => message.WorkerInstanceId).HasMaxLength(200);
        builder.Property(message => message.LastAttemptAt);
        builder.Property(message => message.DeadLetterReason);

        builder.HasIndex(message => message.EventId).IsUnique();
        builder.HasIndex(message => new { message.Status, message.NextRetryAt, message.CreatedAt });
        builder.HasIndex(message => message.LockedUntil);
        builder.HasIndex(message => message.CorrelationId);
    }
}
