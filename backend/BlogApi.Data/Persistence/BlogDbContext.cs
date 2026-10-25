using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Persistence;

public sealed class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
  public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<BlogPost> Posts => Set<BlogPost>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
  }
}
