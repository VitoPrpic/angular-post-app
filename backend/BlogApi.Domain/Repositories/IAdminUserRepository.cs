using BlogApi.Domain.Entities;

namespace BlogApi.Domain.Repositories;

public interface IAdminUserRepository
{
  Task<IReadOnlyList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(
      string username,
      string email,
      CancellationToken cancellationToken = default);
  Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
  Task AddAsync(AdminUser user, CancellationToken cancellationToken = default);
}
