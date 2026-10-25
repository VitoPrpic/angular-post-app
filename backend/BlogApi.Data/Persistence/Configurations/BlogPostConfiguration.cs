using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Data.Persistence.Configurations;

public sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
  public void Configure(EntityTypeBuilder<BlogPost> entity)
  {
    entity.ToTable("Posts");
    entity.HasKey(post => post.Id);
    entity.Property(post => post.Id).HasColumnName("id");
    entity.Property(post => post.AdminUserId).HasColumnName("admin_user_id");
    entity.Property(post => post.CategoryId).HasColumnName("category_id");
    entity.Property(post => post.Title).HasColumnName("title").IsRequired();
    entity.Property(post => post.Content).HasColumnName("content").IsRequired();
    entity.Property(post => post.ImageUrlsJson).HasColumnName("image_urls").HasDefaultValue("[]");
    entity.Property(post => post.CreatedAt)
        .HasColumnName("created_at")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");

    entity.HasOne(post => post.AdminUser)
        .WithMany(user => user.Posts)
        .HasForeignKey(post => post.AdminUserId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(post => post.Category)
        .WithMany(category => category.Posts)
        .HasForeignKey(post => post.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
