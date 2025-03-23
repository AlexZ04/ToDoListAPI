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

        public async Task<IdResponseModel> CreateTask(TaskModel task)
        {
            TaskEntity newTask = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Name = task.Name,
                Description = task.Description,
                Deadline = task.Deadline,
                Status = task.Status,
                Priority = task.Priority,
                CreateTime = DateTime.Now.ToUniversalTime(),
                UpdateTime = DateTime.Now.ToUniversalTime()
            };

            await _taskRepository.CreateTask(newTask);

            return new IdResponseModel
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
