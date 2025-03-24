using ToDoList.Common.Enums;
using ToDoList.Common.Models;
using ToDoList.Persistence.Entities;

namespace ToDoList.Persistence.Repositories
{
    public interface ITaskRepository
    {
        public Task CreateTask(TaskEntity task);
        public Task<List<TaskEntity>> GetAllTasks(TaskFilter filter, IsChecked isChecked);
    }
}
