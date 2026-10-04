namespace BlogApi.Dtos;

public sealed record CreateCategoryRequest(string Name);

public sealed record CategoryResponse(int Id, string Name);
