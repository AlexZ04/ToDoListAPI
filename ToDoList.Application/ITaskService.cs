using ToDoList.Common.Enums;
using ToDoList.Common.Models;

namespace ToDoList.Application
{
    public interface ITaskService
    {
        public Task<IdResponseModel> CreateTask(TaskCreateModel task);
        public Task<TaskListModel> GetAllTasks(TaskFilter filter, IsChecked isChecked);
        public Task<TaskModel> GetTaskInfo(Guid id);
    }
}
