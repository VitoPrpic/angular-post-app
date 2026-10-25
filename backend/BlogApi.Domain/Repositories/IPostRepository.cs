using BlogApi.Domain.Entities;

namespace BlogApi.Domain.Repositories;

public interface IPostRepository
{
  Task<IReadOnlyList<BlogPost>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task AddAsync(BlogPost post, CancellationToken cancellationToken = default);
  Task UpdateAsync(BlogPost post, CancellationToken cancellationToken = default);
  Task DeleteAsync(BlogPost post, CancellationToken cancellationToken = default);
}
