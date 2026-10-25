using BlogApi.Domain.Entities;

namespace BlogApi.Application.Security;

public interface IPasswordHashingService
{
  string HashPassword(AdminUser user, string password);
}
