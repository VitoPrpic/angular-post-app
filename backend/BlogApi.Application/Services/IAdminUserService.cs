using BlogApi.Application.Dtos;

namespace BlogApi.Application.Services;

public interface IAdminUserService
{
  Task<IReadOnlyList<AdminUserResponse>> GetAllAsync(
      CancellationToken cancellationToken = default);
  Task<AdminUserResponse> CreateAsync(
      CreateAdminUserRequest request,
      CancellationToken cancellationToken = default);
}
