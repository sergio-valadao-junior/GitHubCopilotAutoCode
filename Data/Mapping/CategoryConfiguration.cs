using GitHubCopilotAutoCode.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitHubCopilotAutoCode.Data.Mapping;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Entity configuration for Category
        builder
            .HasKey(c => c.Id);


    }
}
