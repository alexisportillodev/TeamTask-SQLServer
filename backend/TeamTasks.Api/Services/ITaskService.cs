using TeamTasks.Api.DTOs;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Services;

public interface ITaskService
{
    Task<ServiceResult<TaskItem>> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct);
    Task<List<TaskItem>> GetTasksAsync(CancellationToken ct);
    Task<ServiceResult<TaskItem>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto, CancellationToken ct);
}

