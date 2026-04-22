using System.ComponentModel.DataAnnotations;

namespace TeamTasks.Api.Models;

public sealed class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<TaskItem> Tasks { get; set; } = [];
}

