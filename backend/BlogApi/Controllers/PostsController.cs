using BlogApi.Application.Dtos;
using BlogApi.Application.Services;
using BlogApi.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/posts")]
public sealed class PostsController(IPostService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<PostResponse>>> GetAll(
      CancellationToken cancellationToken)
  {
    return Ok(await service.GetAllAsync(cancellationToken));
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult<PostResponse>> GetById(
      int id,
      CancellationToken cancellationToken)
  {
    return Ok(await service.GetByIdAsync(id, cancellationToken));
  }

  [HttpPost]
  [ApiKeyAuth]
  public async Task<ActionResult<PostResponse>> Create(
      CreatePostRequest request,
      CancellationToken cancellationToken)
  {
    var response = await service.CreateAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
  }

  [HttpPut("{id:int}")]
  [ApiKeyAuth]
  public async Task<ActionResult<PostResponse>> Update(
      int id,
      UpdatePostRequest request,
      CancellationToken cancellationToken)
  {
    return Ok(await service.UpdateAsync(id, request, cancellationToken));
  }

  [HttpDelete("{id:int}")]
  [ApiKeyAuth]
  public async Task<IActionResult> Delete(
      int id,
      CancellationToken cancellationToken)
  {
    await service.DeleteAsync(id, cancellationToken);
    return NoContent();
  }
}
