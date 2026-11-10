using taskmanager_api.Entities;

namespace taskmanager_api.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public int UserId { get; set; }          // foreign key column
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } // ? means optional (nullable)
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly? DueDate { get; set; }   // a date, not a date+time; optional
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property: the "one" side
    public User User { get; set; } = null!;

    //Enum
    public enum TaskItemStatus
{
    Todo,
    InProgress,
    Done
}

public enum TaskPriority
{
    Low,
    Medium,
    High
}
}