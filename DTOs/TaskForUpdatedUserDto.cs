using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class TaskForUpdatedUserDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Priority { get; set; } = TaskPriorityEnum.Medium.ToString();

        public DateTime? DueDate { get; set; }

        public string? Status { get; set; } = TaskStatusEnum.New.ToString();
    }
}
