using taskmanager_api.Entities;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.DTOs;

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateOnly? DueDate { get; set; }
}