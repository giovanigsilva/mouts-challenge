using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.ConfigureBaseEntity();
        builder.Property(sale => sale.SaleNumber).IsRequired().HasMaxLength(60);
        builder.Property(sale => sale.SaleDate).IsRequired();
        builder.Property(sale => sale.CustomerExternalId).IsRequired();
        builder.Property(sale => sale.CustomerName).IsRequired().HasMaxLength(120);
        builder.Property(sale => sale.BranchExternalId).IsRequired();
        builder.Property(sale => sale.BranchName).IsRequired().HasMaxLength(120);
        builder.Property(sale => sale.TotalAmount).HasPrecision(18, 2);
        builder.Property(sale => sale.IsCancelled).IsRequired();
        builder.Property(sale => sale.CreatedByUserId).IsRequired();
        builder.Property(sale => sale.UpdatedByUserId);
        builder.Property(sale => sale.CancelledByUserId);

        builder.Ignore(sale => sale.DomainEvents);

        builder.HasIndex(sale => sale.SaleNumber).IsUnique();
        builder.HasIndex(sale => sale.CustomerExternalId);
        builder.HasIndex(sale => sale.BranchExternalId);
        builder.HasIndex(sale => sale.SaleDate);
        builder.HasIndex(sale => sale.IsCancelled);
        builder.HasIndex(sale => sale.CreatedByUserId);
        builder.HasIndex(sale => sale.UpdatedByUserId);
        builder.HasIndex(sale => sale.CancelledByUserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sale => sale.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sale => sale.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sale => sale.CancelledByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sale => sale.Items)
            .WithOne()
            .HasForeignKey(item => item.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(sale => sale.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
