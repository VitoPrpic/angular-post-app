using BlogApi.Application.Dtos;
using BlogApi.Application.Exceptions;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;

namespace BlogApi.Application.Services;

public sealed class CategoryService(ICategoryRepository repository) : ICategoryService
{
  public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    var categories = await repository.GetAllAsync(cancellationToken);
    return categories.Select(category => new CategoryResponse(category.Id, category.Name)).ToList();
  }

  public async Task<CategoryResponse> CreateAsync(
      CreateCategoryRequest request,
      CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
    {
      throw new RequestValidationException("Naziv kategorije je obavezan.");
    }

    var name = request.Name.Trim();
    if (await repository.ExistsAsync(name, cancellationToken))
    {
      throw new ResourceConflictException("Kategorija već postoji.");
    }

    var category = new Category { Name = name };
    await repository.AddAsync(category, cancellationToken);
    return new CategoryResponse(category.Id, category.Name);
  }
}
