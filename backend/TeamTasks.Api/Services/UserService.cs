using TeamTasks.Api.DTOs;
using TeamTasks.Api.Models;
using TeamTasks.Api.Repositories;

namespace TeamTasks.Api.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<ServiceResult<User>> CreateUserAsync(CreateUserDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return ServiceResult<User>.Fail("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return ServiceResult<User>.Fail("Email is required.");
        }

        var email = dto.Email.Trim();
        if (await _users.EmailExistsAsync(email, ct))
        {
            return ServiceResult<User>.Fail("Email already exists.");
        }

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
        };

        var created = await _users.AddAsync(user, ct);
        return ServiceResult<User>.Ok(created);
    }

    public Task<List<User>> GetUsersAsync(CancellationToken ct)
    {
        return _users.GetAllAsync(ct);
    }
}

