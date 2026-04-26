using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        builder.HasKey(i => i.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(255);
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(i => i.Discount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(i => i.IsCancelled).HasDefaultValue(false);
    }
}