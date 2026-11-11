using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskmanager_api.DTOs;
using taskmanager_api.Entities;
using taskmanager_api.Services.Interfaces;
using static taskmanager_api.Entities.TaskItem;

namespace taskmanager_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // The logged-in user's id, read from the JWT "sub" claim
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] TaskItemStatus? status,
        [FromQuery] string? search)
    {
        var tasks = await _taskService.GetAllAsync(CurrentUserId, status, search);
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(CurrentUserId, id);

        if (task is null)
        {
            return NotFound(new { message = "Task not found." });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var created = await _taskService.CreateAsync(CurrentUserId, dto);

        // 201 plus a Location header pointing at the new task's URL
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
    {
        var updated = await _taskService.UpdateAsync(CurrentUserId, id, dto);

        if (updated is null)
        {
            return NotFound(new { message = "Task not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(CurrentUserId, id);

        if (!deleted)
        {
            return NotFound(new { message = "Task not found." });
        }

        return NoContent();
    }
}