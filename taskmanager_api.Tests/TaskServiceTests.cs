using Moq;
using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using taskmanager_api.Exceptions;
using taskmanager_api.Repositories.Interfaces;
using taskmanager_api.Services;
using Xunit;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repo = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        // A fresh instance per test, so every test gets its own clean mock
        _service = new TaskService(_repo.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_SetsOwnerAndTrimsText()
    {
        // Arrange: capture whatever entity the service hands to the repository
        TaskItem? saved = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
             .Callback<TaskItem>(t => saved = t)
             .ReturnsAsync((TaskItem t) => t);

        var dto = new CreateTaskDto
        {
            Title = "  Finish API  ",
            Description = "  details  ",
            Priority = TaskPriority.High
        };

        // Act
        var result = await _service.CreateAsync(7, dto);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal(7, saved!.UserId);            // owner comes from the caller, not the DTO
        Assert.Equal("Finish API", saved.Title);   // trimmed
        Assert.Equal("details", saved.Description);
        Assert.Equal("Finish API", result.Title);
        Assert.Equal(TaskPriority.High, result.Priority);
    }

    [Fact]
    public async Task GetAllAsync_PassesFiltersToRepository()
    {
        _repo.Setup(r => r.GetAllByUserAsync(1, TaskItemStatus.Done, "report"))
             .ReturnsAsync(new List<TaskItem> { MakeTask(id: 10) });

        var result = await _service.GetAllAsync(1, TaskItemStatus.Done, "report");

        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_TaskExists_ReturnsDto()
    {
        _repo.Setup(r => r.GetByIdAsync(5, 1)).ReturnsAsync(MakeTask(id: 5, userId: 1));

        var result = await _service.GetByIdAsync(1, 5);

        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_TaskNotFound_ThrowsNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(99, 1)).ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1, 99));
    }

    [Fact]
    public async Task UpdateAsync_TaskExists_UpdatesFieldsAndRefreshesUpdatedAt()
    {
        var oldTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var existing = MakeTask(id: 5, userId: 1);
        existing.UpdatedAt = oldTime;
        _repo.Setup(r => r.GetByIdAsync(5, 1)).ReturnsAsync(existing);

        var dto = new UpdateTaskDto
        {
            Title = " New title ",
            Status = TaskItemStatus.Done,
            Priority = TaskPriority.Low
        };

        var result = await _service.UpdateAsync(1, 5, dto);

        Assert.Equal("New title", result.Title);
        Assert.Equal(TaskItemStatus.Done, result.Status);
        Assert.True(result.UpdatedAt > oldTime);
        _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_TaskNotFound_ThrowsAndNeverSaves()
    {
        _repo.Setup(r => r.GetByIdAsync(99, 1)).ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(1, 99, new UpdateTaskDto { Title = "x" }));

        _repo.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_TaskExists_DeletesIt()
    {
        var existing = MakeTask(id: 5, userId: 1);
        _repo.Setup(r => r.GetByIdAsync(5, 1)).ReturnsAsync(existing);

        await _service.DeleteAsync(1, 5);

        _repo.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_TaskNotFound_ThrowsAndNeverDeletes()
    {
        _repo.Setup(r => r.GetByIdAsync(99, 1)).ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(1, 99));

        _repo.Verify(r => r.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    private static TaskItem MakeTask(int id = 1, int userId = 1) => new()
    {
        Id = id,
        UserId = userId,
        Title = "Sample",
        Status = TaskItemStatus.Todo,
        Priority = TaskPriority.Medium,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}