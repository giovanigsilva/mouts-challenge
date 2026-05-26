using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(item => item.SaleId).HasColumnType("uuid").IsRequired();
        builder.Property(item => item.ProductExternalId).HasColumnType("uuid").IsRequired();
        builder.Property(item => item.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(item => item.Quantity).IsRequired();
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.DiscountPercentage).HasPrecision(5, 2).IsRequired();
        builder.Property(item => item.DiscountAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.IsCancelled).IsRequired();
        builder.Property(item => item.CreatedAt).IsRequired();
        builder.Property(item => item.UpdatedAt);

        builder.HasIndex(item => item.SaleId);
        builder.HasIndex(item => item.ProductExternalId);
    }
}
