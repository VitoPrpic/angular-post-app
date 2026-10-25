using BlogApi.Application.Dtos;

namespace BlogApi.Application.Services;

public interface ICategoryService
{
  Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
      CancellationToken cancellationToken = default);
  Task<CategoryResponse> CreateAsync(
      CreateCategoryRequest request,
      CancellationToken cancellationToken = default);
}
