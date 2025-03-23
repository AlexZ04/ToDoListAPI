using ToDoList.Common.Models;

namespace ToDoList.Application
{
    public interface ITaskService
    {
        public Task<IdResponseModel> CreateTask(TaskModel task);
    }
}
