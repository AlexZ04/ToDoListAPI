using System.Threading.Tasks;
using ToDoList.Common.Enums;
using ToDoList.Common.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Entities;
using ToDoList.Persistence.Repositories;

namespace ToDoList.Application.Implementations
{
    public class TaskServiceImpl : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskServiceImpl(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IdResponseModel> CreateTask(TaskCreateModel task)
        {
            TaskEntity newTask = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Name = task.Name,
                Description = task.Description,
                Deadline = task.Deadline,
                Status = Status.Active,
                Priority = task.Priority,
                IsChecked = false,
                CreateTime = DateTime.Now.ToUniversalTime(),
                UpdateTime = DateTime.Now.ToUniversalTime()
            };

            await _taskRepository.CreateTask(newTask);

            return new IdResponseModel
            {
                Id = newTask.Id
            };
        }

        public async Task<TaskListModel> GetAllTasks(TaskFilter filter, IsChecked isChecked)
        {
            var tasks = await _taskRepository.GetAllTasks(filter, isChecked);

            List<TaskModel> list = new List<TaskModel>();

            foreach (var task in tasks)
                list.Add(new TaskModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    Deadline = task.Deadline,
                    Priority = task.Priority,
                    Status = task.Status,
                    IsChecked = task.IsChecked,
                    CreateTime = task.CreateTime,
                    UpdateTime = task.UpdateTime,
                });

            return new TaskListModel
            {
                Tasks = list
            };
        }
    }
}
