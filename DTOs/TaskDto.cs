using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class TaskDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Priority { get; set; } = TaskPriorityEnum.Medium.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; } = TaskStatusEnum.New.ToString();
    }
}
