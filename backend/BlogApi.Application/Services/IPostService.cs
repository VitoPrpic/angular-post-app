using BlogApi.Application.Dtos;

namespace BlogApi.Application.Services;

public interface IPostService
{
  Task<IReadOnlyList<PostResponse>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<PostResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<PostResponse> CreateAsync(
      CreatePostRequest request,
      CancellationToken cancellationToken = default);
  Task<PostResponse> UpdateAsync(
      int id,
      UpdatePostRequest request,
      CancellationToken cancellationToken = default);
  Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
