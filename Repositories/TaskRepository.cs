using Microsoft.EntityFrameworkCore;
using taskmanager_api.Data;
using taskmanager_api.Entities;
using taskmanager_api.Repositories.Interfaces;

namespace taskmanager_api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskItem>> GetAllByUserAsync(int userId, TaskItem.TaskItemStatus? status, string? search)
    {
        // Start with "only this user's tasks", then add filters one by one.
        // Nothing hits the database until ToListAsync() at the end.
        var query = _db.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term));
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        // Matching on BOTH id and userId: someone else's task simply "doesn't exist" for you
        return await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task; // now has its generated Id
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    // public Task<List<TaskItem>> GetAllByUserAsync(int userId, TaskItem.TaskItemStatus? status, string? search)
    // {
    //     throw new NotImplementedException();
    // }
}