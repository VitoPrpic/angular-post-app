using System.Text.Json;
using BlogApi.Application.Dtos;
using BlogApi.Application.Exceptions;
using BlogApi.Domain.Entities;
using BlogApi.Domain.Repositories;

namespace BlogApi.Application.Services;

public sealed class PostService(
    IPostRepository postRepository,
    IAdminUserRepository adminUserRepository,
    ICategoryRepository categoryRepository) : IPostService
{
  public async Task<IReadOnlyList<PostResponse>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    var posts = await postRepository.GetAllAsync(cancellationToken);
    return posts.Select(ToResponse).ToList();
  }

  public async Task<PostResponse> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default)
  {
    var post = await postRepository.GetByIdAsync(id, cancellationToken);
    if (post is null)
    {
      throw new ResourceNotFoundException("Post ne postoji.");
    }

    return ToResponse(post);
  }

  public async Task<PostResponse> CreateAsync(
      CreatePostRequest request,
      CancellationToken cancellationToken = default)
  {
    var images = await ValidateRequestAsync(
        request.AdminUserId,
        request.CategoryId,
        request.Title,
        request.Content,
        request.ImageUrls,
        cancellationToken);

    var post = new BlogPost
    {
      AdminUserId = request.AdminUserId,
      CategoryId = request.CategoryId,
      Title = request.Title.Trim(),
      Content = request.Content.Trim(),
      ImageUrlsJson = JsonSerializer.Serialize(images),
      CreatedAt = DateTime.UtcNow
    };

    await postRepository.AddAsync(post, cancellationToken);
    return ToResponse(post);
  }

  public async Task<PostResponse> UpdateAsync(
      int id,
      UpdatePostRequest request,
      CancellationToken cancellationToken = default)
  {
    var post = await postRepository.GetByIdAsync(id, cancellationToken);
    if (post is null)
    {
      throw new ResourceNotFoundException("Post ne postoji.");
    }

    var images = await ValidateRequestAsync(
        request.AdminUserId,
        request.CategoryId,
        request.Title,
        request.Content,
        request.ImageUrls,
        cancellationToken);

    post.AdminUserId = request.AdminUserId;
    post.CategoryId = request.CategoryId;
    post.Title = request.Title.Trim();
    post.Content = request.Content.Trim();
    post.ImageUrlsJson = JsonSerializer.Serialize(images);

    await postRepository.UpdateAsync(post, cancellationToken);
    return ToResponse(post);
  }

  public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
  {
    var post = await postRepository.GetByIdAsync(id, cancellationToken);
    if (post is null)
    {
      throw new ResourceNotFoundException("Post ne postoji.");
    }

    await postRepository.DeleteAsync(post, cancellationToken);
  }

  private async Task<List<string>> ValidateRequestAsync(
      int adminUserId,
      int? categoryId,
      string title,
      string content,
      List<string>? imageUrls,
      CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
    {
      throw new RequestValidationException("Naslov i sadržaj su obavezni.");
    }

    var images = CleanImages(imageUrls);
    if (images.Count > 8)
    {
      throw new RequestValidationException("Post može imati najviše 8 slika.");
    }

    if (!await adminUserRepository.ExistsByIdAsync(adminUserId, cancellationToken))
    {
      throw new RequestValidationException("Administrator ne postoji.");
    }

    if (categoryId.HasValue &&
        !await categoryRepository.ExistsByIdAsync(categoryId.Value, cancellationToken))
    {
      throw new RequestValidationException("Kategorija ne postoji.");
    }

    return images;
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
      post.CreatedAt);

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
