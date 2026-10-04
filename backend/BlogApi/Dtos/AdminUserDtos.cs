namespace BlogApi.Dtos;

public sealed record CreateAdminUserRequest(string Username, string Email, string Password);

public sealed record AdminUserResponse(int Id, string Username, string Email);
