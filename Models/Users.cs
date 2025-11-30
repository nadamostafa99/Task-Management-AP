namespace TaskManagerAPI.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } 
        public string? ProfilePictureUrl { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passswordSalt { get; set; }
        public string Role { get; set; } = "User";
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
