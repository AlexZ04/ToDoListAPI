using Microsoft.EntityFrameworkCore;
using ToDoList.Common.Constants;
using ToDoList.Common.Enums;
using ToDoList.Common.Models;
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

        public async Task<List<TaskEntity>> GetAllTasks(TaskFilter filter, IsChecked isChecked)
        {
            var tasks = _context.Tasks
                .AsNoTracking();

            switch (isChecked)
            {
                case IsChecked.Checked:
                    tasks = tasks.Where(t => t.IsChecked);
                    break;

                case IsChecked.NotChecked:
                    tasks = tasks.Where(t => !t.IsChecked);
                    break;
            }


            switch (filter)
            {
                case TaskFilter.CreateTimeDesc:
                    tasks = tasks.OrderByDescending(t => t.CreateTime);
                    break;

                case TaskFilter.CreateTimeAsc:
                    tasks = tasks.OrderBy(t => t.CreateTime);
                    break;

                case TaskFilter.StatusDesc:
                    tasks = tasks.OrderByDescending(t => t.Status);
                    break;

                case TaskFilter.StatusAsc:
                    tasks = tasks.OrderBy(t => t.Status);
                    break;

                case TaskFilter.PriorityDesc:
                    tasks = tasks.OrderByDescending(t => t.Priority);
                    break;

                case TaskFilter.PrioriyAsc:
                    tasks = tasks.OrderBy(t => t.Priority);
                    break;

                case TaskFilter.UpdateTimeDesc:
                    tasks = tasks.OrderByDescending(t => t.UpdateTime);
                    break;

                case TaskFilter.UpdateTimeAsc:
                    tasks = tasks.OrderBy(t => t.UpdateTime);
                    break;
            }

            return await tasks.ToListAsync();
        }

        public async Task<TaskEntity> GetTaskById(Guid id)
        {
            return await _context.Tasks.FindAsync(id) ?? throw new KeyNotFoundException(ErrorMessages.TASK_NOT_FOUND);
        }
        public async Task DeleteTask(Guid id)
        {
            var task = await GetTaskById(id);

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
