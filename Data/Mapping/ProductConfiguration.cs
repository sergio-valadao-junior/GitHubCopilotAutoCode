using GitHubCopilotAutoCode.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitHubCopilotAutoCode.Data.Mapping;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Entity configuration for Product
        builder
            .HasKey(p => p.Id);

        // Configure decimal precision for Price
        builder
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // Configure Category-Product relationship
        builder
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .IsRequired();

    }
}
