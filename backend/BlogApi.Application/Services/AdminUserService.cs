using BlogApi.Application.Dtos;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Security;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;

namespace BlogApi.Application.Services;

public sealed class AdminUserService(
    IAdminUserRepository repository,
    IPasswordHashingService passwordHashingService) : IAdminUserService
{
  public async Task<IReadOnlyList<AdminUserResponse>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    var users = await repository.GetAllAsync(cancellationToken);
    return users.Select(user => new AdminUserResponse(user.Id, user.Username, user.Email)).ToList();
  }

  public async Task<AdminUserResponse> CreateAsync(
      CreateAdminUserRequest request,
      CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
      throw new RequestValidationException(
          "Korisničko ime, email i lozinka su obavezni.");
    }

    var username = request.Username.Trim();
    var email = request.Email.Trim();

    if (await repository.ExistsAsync(username, email, cancellationToken))
    {
      throw new ResourceConflictException("Korisničko ime ili email već postoji.");
    }

    var user = new AdminUser { Username = username, Email = email };
    user.PasswordHash = passwordHashingService.HashPassword(user, request.Password);

    await repository.AddAsync(user, cancellationToken);
    return new AdminUserResponse(user.Id, user.Username, user.Email);
  }
}
