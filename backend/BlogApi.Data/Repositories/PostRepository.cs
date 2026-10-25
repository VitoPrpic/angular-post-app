using BlogApi.Data.Persistence;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Repositories;

public sealed class PostRepository(BlogDbContext database) : IPostRepository
{
  public async Task<IReadOnlyList<BlogPost>> GetAllAsync(
      CancellationToken cancellationToken = default) =>
      await database.Posts
          .AsNoTracking()
          .OrderByDescending(post => post.CreatedAt)
          .ToListAsync(cancellationToken);

  public Task<BlogPost?> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default) =>
      database.Posts.FirstOrDefaultAsync(post => post.Id == id, cancellationToken);

  public async Task AddAsync(
      BlogPost post,
      CancellationToken cancellationToken = default)
  {
    database.Posts.Add(post);
    await database.SaveChangesAsync(cancellationToken);
  }

  public async Task UpdateAsync(
      BlogPost post,
      CancellationToken cancellationToken = default)
  {
    await database.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteAsync(
      BlogPost post,
      CancellationToken cancellationToken = default)
  {
    database.Posts.Remove(post);
    await database.SaveChangesAsync(cancellationToken);
  }
}
