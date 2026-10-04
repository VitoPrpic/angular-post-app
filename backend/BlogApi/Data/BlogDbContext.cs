using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data;

public sealed class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
  public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<BlogPost> Posts => Set<BlogPost>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<AdminUser>(entity =>
    {
      entity.ToTable("AdminUsers");
      entity.HasKey(user => user.Id);
      entity.Property(user => user.Id).HasColumnName("id");
      entity.Property(user => user.Username).HasColumnName("username").IsRequired();
      entity.Property(user => user.Email).HasColumnName("email").IsRequired();
      entity.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
      entity.HasIndex(user => user.Username).IsUnique();
      entity.HasIndex(user => user.Email).IsUnique();
    });

    modelBuilder.Entity<Category>(entity =>
    {
      entity.ToTable("Categories");
      entity.HasKey(category => category.Id);
      entity.Property(category => category.Id).HasColumnName("id");
      entity.Property(category => category.Name).HasColumnName("name").IsRequired();
      entity.HasIndex(category => category.Name).IsUnique();
    });

    modelBuilder.Entity<BlogPost>(entity =>
    {
      entity.ToTable("Posts");
      entity.HasKey(post => post.Id);
      entity.Property(post => post.Id).HasColumnName("id");
      entity.Property(post => post.AdminUserId).HasColumnName("admin_user_id");
      entity.Property(post => post.CategoryId).HasColumnName("category_id");
      entity.Property(post => post.Title).HasColumnName("title").IsRequired();
      entity.Property(post => post.Content).HasColumnName("content").IsRequired();
      entity.Property(post => post.ImageUrlsJson).HasColumnName("image_urls").HasDefaultValue("[]");
      entity.Property(post => post.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

      entity.HasOne(post => post.AdminUser)
              .WithMany(user => user.Posts)
              .HasForeignKey(post => post.AdminUserId)
              .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(post => post.Category)
              .WithMany(category => category.Posts)
              .HasForeignKey(post => post.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);
    });
  }
}
