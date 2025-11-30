using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        
        // GET MY TASKS
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to view your tasks." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var tasks = await _context.Tasks
                    .Where(t => t.userId == userId)
                    .Select(t => new TaskDtoResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Priority = t.Priority.ToString(),
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        DueDate = t.DueDate,
                        Status = t.Status.ToString(),
                        userId = userId
                    })
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to access your tasks." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An error occurred while retrieving tasks: {ex.Message}" });
            }
        }

        
        // GET TASK BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to view task details." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var task = await _context.Tasks.FirstOrDefaultAsync(x => x.userId == userId && x.Id == id);

                if (task == null)
                    return NotFound(new { error = "Task not found." });

                var taskResponse = new TaskDtoResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority.ToString(),
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    DueDate = task.DueDate,
                    Status = task.Status.ToString(),
                    userId = userId
                };

                return Ok(taskResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to view this task." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error retrieving task: {ex.Message}" });
            }
        }

        // CREATE TASK
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskForUserDto taskDto)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to create tasks." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (!Enum.IsDefined(typeof(TaskPriorityEnum), taskDto.Priority))
                    return BadRequest(new { error = "Invalid priority value." });

                if (!Enum.IsDefined(typeof(TaskStatusEnum), taskDto.Status))
                    return BadRequest(new { error = "Invalid status value." });

                var task = new TaskItem
                {
                    userId = userId,
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Priority = taskDto.Priority,
                    Status = taskDto.Status,
                    DueDate = taskDto.DueDate
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var response = new TaskDtoResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority.ToString(),
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    DueDate = task.DueDate,
                    Status = task.Status.ToString(),
                    userId = userId
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to create tasks." });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = $"Database write error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }

        
        // UPDATE TASK
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskForUserDto task)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to update tasks." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.userId == userId);

                if (existingTask == null)
                    return NotFound(new { error = "Task not found." });

                if (task.Title != null)
                    existingTask.Title = task.Title;

                if (task.Description != null)
                    existingTask.Description = task.Description;

                if (task.Priority != null)
                    existingTask.Priority = task.Priority;

                if (task.DueDate != null)
                    existingTask.DueDate = task.DueDate;

                if (task.Status != null)
                    existingTask.Status = task.Status;

                existingTask.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new TaskDtoResponse
                {
                    Id = existingTask.Id,
                    Title = existingTask.Title,
                    Description = existingTask.Description,
                    Priority = existingTask.Priority.ToString(),
                    CreatedAt = existingTask.CreatedAt,
                    UpdatedAt = existingTask.UpdatedAt,
                    DueDate = existingTask.DueDate,
                    Status = existingTask.Status.ToString(),
                    userId = userId
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to update this task." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { error = "Another user updated this task at the same time. Try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }

        
        // DELETE TASK
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to delete tasks." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.userId == userId);

                if (task == null)
                    return NotFound(new { error = "Task not found." });

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Task deleted successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to delete tasks." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }
    }
}
