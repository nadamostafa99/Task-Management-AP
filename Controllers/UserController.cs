using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ======================================
        // REGISTER
        // ======================================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            try
            {
                // Email already exists?
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return BadRequest("User already exists.");

                PasswordHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

                var user = new Users()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    Nationality = request.Nationality,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    ZipCode = request.ZipCode,
                    EmergencyContact = request.EmergencyContact,
                    passwordHash = hash,
                    passswordSalt = salt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error while creating the user: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        // ======================================
        // LOGIN
        // ======================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

                if (user == null)
                    return BadRequest("User not found.");

                if (!PasswordHelper.VerifyPasswordHash(login.password, user.passwordHash, user.passswordSalt))
                    return BadRequest("Wrong password.");

                var token = JwtHelper.GenerateToken(user, _configuration);

                return Ok(new { token, user.Role });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
