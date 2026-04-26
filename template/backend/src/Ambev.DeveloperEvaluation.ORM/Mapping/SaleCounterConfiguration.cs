using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Entities.Counters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations;

public class SaleCounterConfiguration : IEntityTypeConfiguration<SaleCounter>
{
    public void Configure(EntityTypeBuilder<SaleCounter> builder)
    {
        builder.ToTable("SaleCounters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Date).IsRequired().HasColumnType("date");
        builder.Property(c => c.LastNumber).IsRequired();

        builder.HasIndex(c => c.Date).IsUnique();
    }
}