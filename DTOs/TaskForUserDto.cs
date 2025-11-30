using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class TaskForUserDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Priority { get; set; } = TaskPriorityEnum.Medium.ToString();

        public DateTime? DueDate { get; set; }

        public string? Status { get; set; } = TaskStatusEnum.New.ToString();
    }
}
