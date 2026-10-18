using BlogApi.Authentication;
using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/admin-users")]
[ApiKeyAuth]
public sealed class AdminUsersController(
    BlogDbContext database,
    IPasswordHasher<AdminUser> passwordHasher) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<AdminUserResponse>>> GetAll()
  {
    var users = await database.AdminUsers
        .AsNoTracking()
        .OrderBy(user => user.Username)
        .Select(user => new AdminUserResponse(user.Id, user.Username, user.Email))
        .ToListAsync();

    return Ok(users);
  }

  [HttpPost]
  public async Task<ActionResult<AdminUserResponse>> Create(CreateAdminUserRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
      return BadRequest(new { message = "Korisničko ime, email i lozinka su obavezni." });
    }

    var username = request.Username.Trim();
    var email = request.Email.Trim();

    if (await database.AdminUsers.AnyAsync(user => user.Username == username || user.Email == email))
    {
      return Conflict(new { message = "Korisničko ime ili email već postoji." });
    }

    var user = new AdminUser { Username = username, Email = email };
    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

    database.AdminUsers.Add(user);
    await database.SaveChangesAsync();

    var response = new AdminUserResponse(user.Id, user.Username, user.Email);
    return Created($"/api/admin-users/{user.Id}", response);
  }
}
