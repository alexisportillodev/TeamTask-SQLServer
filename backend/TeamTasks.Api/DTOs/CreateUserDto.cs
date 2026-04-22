using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.DTOs;

public sealed class CreateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

