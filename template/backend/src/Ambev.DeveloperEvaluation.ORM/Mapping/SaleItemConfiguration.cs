using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.ConfigureBaseEntity();
        builder.Property(item => item.SaleId).IsRequired();
        builder.Property(item => item.ProductExternalId).IsRequired();
        builder.Property(item => item.ProductName).IsRequired().HasMaxLength(120);
        builder.Property(item => item.Quantity).IsRequired();
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Property(item => item.DiscountPercentage).HasPrecision(5, 2);
        builder.Property(item => item.DiscountAmount).HasPrecision(18, 2);
        builder.Property(item => item.TotalAmount).HasPrecision(18, 2);
        builder.Property(item => item.IsCancelled).IsRequired();

        builder.HasIndex(item => item.SaleId);
        builder.HasIndex(item => item.ProductExternalId);
    }
}
