using taskmanager_api.Entities;

namespace taskmanager_api.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllByUserAsync(int userId, TaskItem.TaskItemStatus? status, string? search);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}