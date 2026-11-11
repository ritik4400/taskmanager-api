using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.Services.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponseDto>> GetAllAsync(int userId, TaskItemStatus? status, string? search);
    Task<TaskResponseDto?> GetByIdAsync(int userId, int taskId);
    Task<TaskResponseDto> CreateAsync(int userId, CreateTaskDto dto);
    Task<TaskResponseDto?> UpdateAsync(int userId, int taskId, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int userId, int taskId);
}