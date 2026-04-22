using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.DTOs;

public sealed class UpdateTaskStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}

