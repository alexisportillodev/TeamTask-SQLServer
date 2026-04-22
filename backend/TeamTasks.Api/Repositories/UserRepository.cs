using Microsoft.EntityFrameworkCore;
using TeamTasks.Api.Data;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly TeamTasksDbContext _db;

    public UserRepository(TeamTasksDbContext db)
    {
        _db = db;
    }

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        return _db.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return _db.Users.AnyAsync(x => x.Email == email, ct);
    }
}

