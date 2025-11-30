using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class UpdatedUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } 
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Role { get; set; } 
        public ICollection<TaskForUpdatedUserDto> Tasks { get; set; } = new List<TaskForUpdatedUserDto>();

    }
}
