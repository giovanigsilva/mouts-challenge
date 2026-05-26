using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(sale => sale.Id);
        builder.Property(sale => sale.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(sale => sale.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(sale => sale.SaleDate).IsRequired();
        builder.Property(sale => sale.CustomerExternalId).HasColumnType("uuid").IsRequired();
        builder.Property(sale => sale.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(sale => sale.BranchExternalId).HasColumnType("uuid").IsRequired();
        builder.Property(sale => sale.BranchName).IsRequired().HasMaxLength(200);
        builder.Property(sale => sale.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(sale => sale.IsCancelled).IsRequired();
        builder.Property(sale => sale.CreatedAt).IsRequired();
        builder.Property(sale => sale.UpdatedAt);

        builder.HasIndex(sale => sale.SaleNumber).IsUnique();
        builder.HasIndex(sale => sale.CustomerExternalId);
        builder.HasIndex(sale => sale.BranchExternalId);
        builder.HasIndex(sale => sale.SaleDate);
        builder.HasIndex(sale => sale.IsCancelled);

        builder.HasMany(sale => sale.Items)
            .WithOne()
            .HasForeignKey(item => item.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(sale => sale.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(sale => sale.DomainEvents);
    }
}
