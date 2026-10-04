using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(BlogDbContext database) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<CategoryResponse>>> GetAll()
  {
    var categories = await database.Categories
        .AsNoTracking()
        .OrderBy(category => category.Name)
        .Select(category => new CategoryResponse(category.Id, category.Name))
        .ToListAsync();

    return Ok(categories);
  }

  [HttpPost]
  public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
    {
      return BadRequest(new { message = "Naziv kategorije je obavezan." });
    }

    var name = request.Name.Trim();
    if (await database.Categories.AnyAsync(category => category.Name == name))
    {
      return Conflict(new { message = "Kategorija već postoji." });
    }

    var category = new Category { Name = name };
    database.Categories.Add(category);
    await database.SaveChangesAsync();

    var response = new CategoryResponse(category.Id, category.Name);
    return Created($"/api/categories/{category.Id}", response);
  }
}
