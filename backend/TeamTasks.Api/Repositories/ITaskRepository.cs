using TeamTasks.Api.Models;

namespace TeamTasks.Api.Repositories;

public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct);
    Task<List<TaskItem>> GetAllAsync(CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

