using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using taskmanager_api.Exceptions;
using taskmanager_api.Repositories.Interfaces;
using taskmanager_api.Services.Interfaces;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskResponseDto>> GetAllAsync(int userId, TaskItemStatus? status, string? search)
    {
        var tasks = await _taskRepository.GetAllByUserAsync(userId, status, search);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskResponseDto?> GetByIdAsync(int userId, int taskId)
    {
        var task = await GetOwnedTaskAsync(taskId, userId);
        return MapToDto(task);
    }

    public async Task<TaskResponseDto> CreateAsync(int userId, CreateTaskDto dto)
    {
        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            UserId = userId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _taskRepository.AddAsync(task);
        return MapToDto(task);
    }

    public async Task<TaskResponseDto?> UpdateAsync(int userId, int taskId, UpdateTaskDto dto)
    {
        var task = await GetOwnedTaskAsync(taskId, userId);

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        return MapToDto(task);
    }

    public async Task<bool> DeleteAsync(int userId, int taskId)
    {
        var task = await GetOwnedTaskAsync(taskId, userId);

        await _taskRepository.DeleteAsync(task);
        return true;
    }

    // The "find it or throw 404" logic lives in one place now
    private async Task<TaskItem> GetOwnedTaskAsync(int userId, int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, userId);

        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        return task;
    }

    // Entity -> DTO. Manual mapping is fine at this size; no AutoMapper needed.
    private static TaskResponseDto MapToDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}