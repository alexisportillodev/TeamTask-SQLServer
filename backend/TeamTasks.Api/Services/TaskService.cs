using System.Text.Json;
using TeamTasks.Api.DTOs;
using TeamTasks.Api.Models;
using TeamTasks.Api.Repositories;
using TaskStatusModel = TeamTasks.Api.Models.TaskStatus;

namespace TeamTasks.Api.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IUserRepository _users;

    public TaskService(ITaskRepository tasks, IUserRepository users)
    {
        _tasks = tasks;
        _users = users;
    }

    public async Task<ServiceResult<TaskItem>> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return ServiceResult<TaskItem>.Fail("Title is required.");
        }

        if (dto.UserId <= 0)
        {
            return ServiceResult<TaskItem>.Fail("UserId is required.");
        }

        var user = await _users.GetByIdAsync(dto.UserId, ct);
        if (user is null)
        {
            return ServiceResult<TaskItem>.Fail("UserId is not valid.");
        }

        var additionalData = dto.AdditionalData;
        if (!string.IsNullOrWhiteSpace(additionalData))
        {
            try
            {
                JsonDocument.Parse(additionalData);
            }
            catch (JsonException)
            {
                return ServiceResult<TaskItem>.Fail("AdditionalData must be valid JSON.");
            }
        }

        var task = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            UserId = dto.UserId,
            Status = TaskStatusModel.Pending, // business rule: initial status is Pending
            AdditionalData = string.IsNullOrWhiteSpace(additionalData) ? null : additionalData,
        };

        var created = await _tasks.AddAsync(task, ct);
        return ServiceResult<TaskItem>.Ok(created);
    }

    public Task<List<TaskItem>> GetTasksAsync(CancellationToken ct)
    {
        return _tasks.GetAllAsync(ct);
    }

    public async Task<ServiceResult<TaskItem>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto, CancellationToken ct)
    {
        if (id <= 0)
        {
            return ServiceResult<TaskItem>.Fail("Invalid task id.");
        }

        if (string.IsNullOrWhiteSpace(dto.Status))
        {
            return ServiceResult<TaskItem>.Fail("Status is required.");
        }

        if (!Enum.TryParse<TaskStatusModel>(dto.Status.Trim(), ignoreCase: true, out var newStatus))
        {
            return ServiceResult<TaskItem>.Fail("Status must be one of: Pending, InProgress, Done.");
        }

        var task = await _tasks.GetByIdAsync(id, ct);
        if (task is null)
        {
            return ServiceResult<TaskItem>.Fail("Task not found.");
        }

        var current = task.Status;
        if (!TaskStatusRules.CanChangeStatus(current, newStatus))
        {
            return ServiceResult<TaskItem>.Fail("Cannot change status from Pending directly to Done.");
        }

        task.Status = newStatus;
        await _tasks.SaveChangesAsync(ct);
        return ServiceResult<TaskItem>.Ok(task);
    }
}

