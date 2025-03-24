using ToDoList.Common.Enums;

namespace ToDoList.Common.Models
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; } = DateTime.UtcNow.ToUniversalTime();
        public Status Status { get; set; } = Status.Active;
        public Priority Priority { get; set; } = Priority.Medium;
        public bool IsChecked { get; set; } = false;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow.ToUniversalTime();
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow.ToUniversalTime();

    }
}
