using BlogApi.Domain.Entities;

namespace BlogApi.Domain.Repositories;

public interface ICategoryRepository
{
  Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
  Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
  Task AddAsync(Category category, CancellationToken cancellationToken = default);
}
