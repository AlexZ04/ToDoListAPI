using ToDoList.Persistence.Entities;

namespace ToDoList.Persistence.Repositories.Implementations
{
    public class TaskRepositoryImpl : ITaskRepository
    {
        private readonly DataContext _context;

        public TaskRepositoryImpl(DataContext context)
        {
            _context = context;
        }

        public async Task CreateTask(TaskEntity task)
        {
            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();
        }
    }
}
