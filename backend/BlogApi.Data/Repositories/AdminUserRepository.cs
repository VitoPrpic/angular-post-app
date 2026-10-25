using BlogApi.Data.Persistence;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Repositories;

public sealed class AdminUserRepository(BlogDbContext database) : IAdminUserRepository
{
  public async Task<IReadOnlyList<AdminUser>> GetAllAsync(
      CancellationToken cancellationToken = default) =>
      await database.AdminUsers
          .AsNoTracking()
          .OrderBy(user => user.Username)
          .ToListAsync(cancellationToken);

  public Task<bool> ExistsAsync(
      string username,
      string email,
      CancellationToken cancellationToken = default) =>
      database.AdminUsers.AnyAsync(
          user => user.Username == username || user.Email == email,
          cancellationToken);

  public Task<bool> ExistsByIdAsync(
      int id,
      CancellationToken cancellationToken = default) =>
      database.AdminUsers.AnyAsync(user => user.Id == id, cancellationToken);

  public async Task AddAsync(
      AdminUser user,
      CancellationToken cancellationToken = default)
  {
    database.AdminUsers.Add(user);
    await database.SaveChangesAsync(cancellationToken);
  }
}
