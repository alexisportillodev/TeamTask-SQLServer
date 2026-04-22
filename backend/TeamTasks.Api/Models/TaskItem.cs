using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.Models;

public sealed class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? AdditionalData { get; set; }
}

