using BlogApi.Application.Dtos;
using BlogApi.Application.Services;
using BlogApi.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICategoryService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
      CancellationToken cancellationToken)
  {
    return Ok(await service.GetAllAsync(cancellationToken));
  }

  [HttpPost]
  [ApiKeyAuth]
  public async Task<ActionResult<CategoryResponse>> Create(
      CreateCategoryRequest request,
      CancellationToken cancellationToken)
  {
    var response = await service.CreateAsync(request, cancellationToken);
    return Created($"/api/categories/{response.Id}", response);
  }
}
