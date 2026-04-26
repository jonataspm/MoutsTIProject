using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.Date).IsRequired();

        builder.OwnsMany(c => c.Products, rb =>
        {
            rb.ToTable("CartItems");
            rb.WithOwner().HasForeignKey("CartId");
            rb.HasKey("CartId", "ProductId");

            rb.Property(p => p.ProductId).IsRequired();
            rb.Property(p => p.Quantity).IsRequired();
        });
    }
}