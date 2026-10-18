using System.Text.Json;
using BlogApi.Authentication;
using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/posts")]
public sealed class PostsController(BlogDbContext database) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<PostResponse>>> GetAll()
  {
    var posts = await database.Posts
        .AsNoTracking()
        .OrderByDescending(post => post.CreatedAt)
        .ToListAsync();

    return Ok(posts.Select(ToResponse).ToList());
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult<PostResponse>> GetById(int id)
  {
    var post = await database.Posts.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
    return post is null ? NotFound() : Ok(ToResponse(post));
  }

  [HttpPost]
  [ApiKeyAuth]
  public async Task<ActionResult<PostResponse>> Create(CreatePostRequest request)
  {
    var validationError = await ValidateRequest(
        request.AdminUserId,
        request.CategoryId,
        request.Title,
        request.Content,
        request.ImageUrls);

    if (validationError is not null)
    {
      return BadRequest(new { message = validationError });
    }

    var post = new BlogPost
    {
      AdminUserId = request.AdminUserId,
      CategoryId = request.CategoryId,
      Title = request.Title.Trim(),
      Content = request.Content.Trim(),
      ImageUrlsJson = JsonSerializer.Serialize(CleanImages(request.ImageUrls)),
      CreatedAt = DateTime.UtcNow
    };

    database.Posts.Add(post);
    await database.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = post.Id }, ToResponse(post));
  }

  [HttpPut("{id:int}")]
  [ApiKeyAuth]
  public async Task<ActionResult<PostResponse>> Update(int id, UpdatePostRequest request)
  {
    var post = await database.Posts.FindAsync(id);
    if (post is null)
    {
      return NotFound();
    }

    var validationError = await ValidateRequest(
        request.AdminUserId,
        request.CategoryId,
        request.Title,
        request.Content,
        request.ImageUrls);

    if (validationError is not null)
    {
      return BadRequest(new { message = validationError });
    }

    post.AdminUserId = request.AdminUserId;
    post.CategoryId = request.CategoryId;
    post.Title = request.Title.Trim();
    post.Content = request.Content.Trim();
    post.ImageUrlsJson = JsonSerializer.Serialize(CleanImages(request.ImageUrls));

    await database.SaveChangesAsync();
    return Ok(ToResponse(post));
  }

  [HttpDelete("{id:int}")]
  [ApiKeyAuth]
  public async Task<IActionResult> Delete(int id)
  {
    var post = await database.Posts.FindAsync(id);
    if (post is null)
    {
      return NotFound();
    }

    database.Posts.Remove(post);
    await database.SaveChangesAsync();
    return NoContent();
  }

  private async Task<string?> ValidateRequest(
      int adminUserId,
      int? categoryId,
      string title,
      string content,
      List<string>? imageUrls)
  {
    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
    {
      return "Naslov i sadržaj su obavezni.";
    }

    if (CleanImages(imageUrls).Count > 8)
    {
      return "Post može imati najviše 8 slika.";
    }

    if (!await database.AdminUsers.AnyAsync(user => user.Id == adminUserId))
    {
      return "Administrator ne postoji.";
    }

    if (categoryId.HasValue &&
        !await database.Categories.AnyAsync(category => category.Id == categoryId.Value))
    {
      return "Kategorija ne postoji.";
    }

    return null;
  }

  private static List<string> CleanImages(List<string>? imageUrls) =>
      imageUrls?
          .Where(url => !string.IsNullOrWhiteSpace(url))
          .Select(url => url.Trim())
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToList() ?? [];

  private static PostResponse ToResponse(BlogPost post) => new(
      post.Id,
      post.AdminUserId,
      post.CategoryId,
      post.Title,
      post.Content,
      ReadImages(post.ImageUrlsJson),
      post.CreatedAt
  );

  private static List<string> ReadImages(string json)
  {
    try
    {
      return JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }
    catch (JsonException)
    {
      return [];
    }
  }
}
