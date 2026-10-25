namespace BlogApi.Application.Dtos;

public sealed record CreatePostRequest(
    int AdminUserId,
    int? CategoryId,
    string Title,
    string Content,
    List<string>? ImageUrls);

public sealed record UpdatePostRequest(
    int AdminUserId,
    int? CategoryId,
    string Title,
    string Content,
    List<string>? ImageUrls);

public sealed record PostResponse(
    int Id,
    int AdminUserId,
    int? CategoryId,
    string Title,
    string Content,
    List<string> ImageUrls,
    DateTime CreatedAt);
