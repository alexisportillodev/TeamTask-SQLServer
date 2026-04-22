using Microsoft.AspNetCore.Mvc;
using TeamTasks.Api.DTOs;
using TeamTasks.Api.Services;

namespace TeamTasks.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _tasks.CreateTaskAsync(dto, ct);
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
            var tasks = await _tasks.GetTasksAsync(ct);
            return Ok(tasks);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." });
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateTaskStatusDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _tasks.UpdateTaskStatusAsync(id, dto, ct);
            if (!result.Success)
            {
                if (string.Equals(result.Error, "Task not found.", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { error = result.Error });
                }

                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." });
        }
    }
}

