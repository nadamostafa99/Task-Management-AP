using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [AdminOnly]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // ------------------------------------
        // GET ALL USERS
        // ------------------------------------
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to view user list." });

                var users = await _context.Users
                    .Include(u => u.Tasks)
                    .Select(u => new UpdatedResponseUser
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Gender = u.Gender,
                        Nationality = u.Nationality,
                        City = u.City,
                        Address = u.Address,
                        State = u.State,
                        Role = u.Role,
                        Tasks = u.Tasks.Select(t => new TaskDto
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Description = t.Description,
                            Priority = t.Priority.ToString(),
                            CreatedAt = t.CreatedAt,
                            UpdatedAt = t.UpdatedAt,
                            DueDate = t.DueDate,
                            Status = t.Status.ToString()
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to view the user list." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error while fetching users: {ex.Message}" });
            }
        }

        // ------------------------------------
        // DELETE USER
        // ------------------------------------
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to delete users." });

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { error = "User not found." });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"User with ID {id} deleted successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to delete users." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }

        // ------------------------------------
        // GET ALL TASKS
        // ------------------------------------
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to view all tasks." });

                var tasks = await _context.Tasks.Select(t => new TaskDtoResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    DueDate = t.DueDate,
                    Status = t.Status.ToString(),
                    userId = t.userId
                })
                .ToListAsync();

                return Ok(tasks);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to view tasks." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error retrieving tasks: {ex.Message}" });
            }
        }

        // ------------------------------------
        // UPDATE USER
        // ------------------------------------
        [HttpPatch("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdatedUserDto updatedUser)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to update users." });

                var user = await _context.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return NotFound(new { error = "User not found." });

                if (updatedUser.FirstName != null) { user.FirstName = updatedUser.FirstName; }
                if (updatedUser.LastName != null) { user.LastName = updatedUser.LastName; }
                if (updatedUser.Email != null) { user.Email = updatedUser.Email; }
                if (updatedUser.PhoneNumber != null) { user.PhoneNumber = updatedUser.PhoneNumber; }
                if (updatedUser.DateOfBirth != null) { user.DateOfBirth = updatedUser.DateOfBirth; }
                if (updatedUser.Gender != null) { user.Gender = updatedUser.Gender; }
                if (updatedUser.Nationality != null) { user.Nationality = updatedUser.Nationality; }
                if (updatedUser.Address != null) { user.Address = updatedUser.Address; }
                if (updatedUser.City != null) { user.City = updatedUser.City; }
                if (updatedUser.State != null) { user.State = updatedUser.State; }
                if (updatedUser.ZipCode != null) { user.ZipCode = updatedUser.ZipCode; }
                if (updatedUser.Role != null) { user.Role = updatedUser?.Role; }
                foreach (var updatedTask in updatedUser.Tasks) { var existingTask = user.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id); if (existingTask != null) { if (updatedTask.Title != null) existingTask.Title = updatedTask.Title; if (updatedTask.Description != null) existingTask.Description = updatedTask.Description; if (updatedTask.Priority != null) existingTask.Priority = updatedTask.Priority; if (updatedTask.DueDate != null) existingTask.DueDate = updatedTask.DueDate; if (updatedTask.Status != null) existingTask.Status = updatedTask.Status; existingTask.UpdatedAt = DateTime.UtcNow; } }

                await _context.SaveChangesAsync();
                var userResponse = new UpdatedResponseUser()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Nationality = user.Nationality,
                    City = user.City,
                    Address = user.Address,
                    State = user.State,
                    Role = user.Role,
                    Tasks = user.Tasks.Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Priority = t.Priority.ToString(),
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        DueDate = t.DueDate,
                        Status = t.Status.ToString()
                    }).ToList()
                };

                return Ok(userResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to update users." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }

        // ------------------------------------
        // CREATE USER
        // ------------------------------------
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto newUser)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? false)
                    return Unauthorized(new { error = "You must be logged in to create new users." });

                if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
                    return BadRequest(new { error = "User already exists." });

                if (string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password)) return BadRequest("Email and Password are required.");

                PasswordHelper.CreatePasswordHash(newUser.Password, out byte[] hash, out byte[] salt);

                var user = new Users()
                {
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    PhoneNumber = newUser.PhoneNumber,
                    DateOfBirth = newUser.DateOfBirth,
                    Gender = newUser.Gender,
                    ProfilePictureUrl = newUser.ProfilePictureUrl,
                    Nationality = newUser.Nationality,
                    Address = newUser.Address,
                    City = newUser.City,
                    State = newUser.State,
                    ZipCode = newUser.ZipCode,
                    EmergencyContact = newUser.EmergencyContact,
                    passwordHash = hash,
                    passswordSalt = salt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User created successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "You are not authorized to create new users." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }
    }
}
