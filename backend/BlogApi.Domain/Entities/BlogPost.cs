namespace BlogApi.Domain.Entities;

public sealed class BlogPost
{
  public int Id { get; set; }
  public int AdminUserId { get; set; }
  public int? CategoryId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public string ImageUrlsJson { get; set; } = "[]";
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public AdminUser AdminUser { get; set; } = null!;
  public Category? Category { get; set; }
}
