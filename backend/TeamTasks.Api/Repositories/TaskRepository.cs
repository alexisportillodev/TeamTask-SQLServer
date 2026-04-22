using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Data;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TeamTasksDbContext _db;

    public TaskRepository(TeamTasksDbContext db)
    {
        _db = db;
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(ct);
        return task;
    }

    public Task<List<TaskItem>> GetAllAsync(CancellationToken ct)
    {
        return _db.Tasks
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        return _db.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}

