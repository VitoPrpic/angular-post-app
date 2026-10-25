using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Data.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
  public void Configure(EntityTypeBuilder<Category> entity)
  {
    entity.ToTable("Categories");
    entity.HasKey(category => category.Id);
    entity.Property(category => category.Id).HasColumnName("id");
    entity.Property(category => category.Name).HasColumnName("name").IsRequired();
    entity.HasIndex(category => category.Name).IsUnique();
  }
}
