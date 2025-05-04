using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ToDoList.Common.Enums;

namespace ToDoList.Common.Models
{
    public class TaskCreateModel
    {
        [Required, MinLength(4)]
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        public string Description { get; set; } = string.Empty;
        [AllowNull]
        public DateTime? Deadline { get; set; } = null;
        [AllowNull]
        public Priority? Priority { get; set; } = null;
    }
}
