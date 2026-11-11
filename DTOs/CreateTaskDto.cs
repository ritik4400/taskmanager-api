using taskmanager_api.Entities;
using static taskmanager_api.Entities.TaskItem;
using System.ComponentModel.DataAnnotations;

namespace taskmanager_api.DTOs;

public class CreateTaskDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskItemStatus))]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly? DueDate { get; set; }
}