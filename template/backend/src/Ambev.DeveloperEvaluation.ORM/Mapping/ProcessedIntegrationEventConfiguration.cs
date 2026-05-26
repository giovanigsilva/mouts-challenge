using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class ProcessedIntegrationEventConfiguration : IEntityTypeConfiguration<ProcessedIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
    {
        builder.ToTable("ProcessedIntegrationEvents");

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(item => item.EventId).HasColumnType("uuid").IsRequired();
        builder.Property(item => item.EventType).IsRequired().HasMaxLength(200);
        builder.Property(item => item.ConsumerName).IsRequired().HasMaxLength(200);
        builder.Property(item => item.ProcessedAt).IsRequired();
        builder.Property(item => item.CorrelationId).HasColumnType("uuid");

        builder.HasIndex(item => new { item.EventId, item.ConsumerName }).IsUnique();
    }
}
