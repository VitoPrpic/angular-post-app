namespace BlogApi.Domain.Entities;

public sealed class Category
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<BlogPost> Posts { get; set; } = [];
}
