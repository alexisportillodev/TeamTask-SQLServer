using TeamTasks.Api.DTOs;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Services;

public interface IUserService
{
    Task<ServiceResult<User>> CreateUserAsync(CreateUserDto dto, CancellationToken ct);
    Task<List<User>> GetUsersAsync(CancellationToken ct);
}

