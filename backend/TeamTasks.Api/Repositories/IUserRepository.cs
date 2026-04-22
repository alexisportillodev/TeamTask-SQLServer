using TeamTasks.Api.Models;

namespace TeamTasks.Api.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user, CancellationToken ct);
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
}

