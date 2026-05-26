using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class OutboxAdminActionConfiguration : IEntityTypeConfiguration<OutboxAdminAction>
{
    public void Configure(EntityTypeBuilder<OutboxAdminAction> builder)
    {
        builder.ToTable("OutboxAdminActions");

        builder.HasKey(action => action.Id);
        builder.Property(action => action.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(action => action.OutboxMessageId).HasColumnType("uuid");
        builder.Property(action => action.Action).IsRequired().HasMaxLength(100);
        builder.Property(action => action.Reason).IsRequired().HasMaxLength(500);
        builder.Property(action => action.PerformedBy).IsRequired().HasMaxLength(200);
        builder.Property(action => action.PerformedAt).IsRequired();
        builder.Property(action => action.CorrelationId).HasColumnType("uuid");
        builder.Property(action => action.PreviousStatus).HasMaxLength(50);
        builder.Property(action => action.NewStatus).HasMaxLength(50);

        builder.HasIndex(action => action.OutboxMessageId);
        builder.HasIndex(action => action.CorrelationId);
    }
}
