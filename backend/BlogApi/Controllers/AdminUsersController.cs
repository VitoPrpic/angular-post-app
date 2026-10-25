using BlogApi.Application.Dtos;
using BlogApi.Application.Services;
using BlogApi.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/admin-users")]
[ApiKeyAuth]
public sealed class AdminUsersController(IAdminUserService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<AdminUserResponse>>> GetAll(
      CancellationToken cancellationToken)
  {
    return Ok(await service.GetAllAsync(cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<AdminUserResponse>> Create(
      CreateAdminUserRequest request,
      CancellationToken cancellationToken)
  {
    var response = await service.CreateAsync(request, cancellationToken);
    return Created($"/api/admin-users/{response.Id}", response);
  }
}
