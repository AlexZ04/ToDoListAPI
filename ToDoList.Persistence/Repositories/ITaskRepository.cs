using ToDoList.Persistence.Entities;

namespace ToDoList.Persistence.Repositories
{
    public interface ITaskRepository
    {
        public Task CreateTask(TaskEntity task);
    }
}
