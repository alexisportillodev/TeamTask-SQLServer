using Microsoft.AspNetCore.Mvc;
using TeamTasks.Api.DTOs;
using TeamTasks.Api.Services;

namespace TeamTasks.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _users.CreateUserAsync(dto, ct);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        try
        {
            var users = await _users.GetUsersAsync(ct);
            return Ok(users);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." });
        }
    }
}

