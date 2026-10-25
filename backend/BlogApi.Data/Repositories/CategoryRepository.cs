using BlogApi.Data.Persistence;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Repositories;

public sealed class CategoryRepository(BlogDbContext database) : ICategoryRepository
{
  public async Task<IReadOnlyList<Category>> GetAllAsync(
      CancellationToken cancellationToken = default) =>
      await database.Categories
          .AsNoTracking()
          .OrderBy(category => category.Name)
          .ToListAsync(cancellationToken);

  public Task<bool> ExistsAsync(
      string name,
      CancellationToken cancellationToken = default) =>
      database.Categories.AnyAsync(category => category.Name == name, cancellationToken);

  public Task<bool> ExistsByIdAsync(
      int id,
      CancellationToken cancellationToken = default) =>
      database.Categories.AnyAsync(category => category.Id == id, cancellationToken);

  public async Task AddAsync(
      Category category,
      CancellationToken cancellationToken = default)
  {
    database.Categories.Add(category);
    await database.SaveChangesAsync(cancellationToken);
  }
}
