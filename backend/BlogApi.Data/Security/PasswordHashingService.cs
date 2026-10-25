using BlogApi.Application.Security;
using BlogApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Data.Security;

public sealed class PasswordHashingService : IPasswordHashingService
{
  private readonly PasswordHasher<AdminUser> passwordHasher = new();

  public string HashPassword(AdminUser user, string password) =>
      passwordHasher.HashPassword(user, password);
}
