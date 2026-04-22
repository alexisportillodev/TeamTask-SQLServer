using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.DTOs;

public sealed class CreateTaskDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int UserId { get; set; }

    public string? AdditionalData { get; set; }
}

