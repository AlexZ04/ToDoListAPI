using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToDoList.Common.Constants;
using ToDoList.Common.Enums;
using ToDoList.Common.Exceptions;
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
                Name = task.Name.Trim(),
                Description = task.Description.Trim(),
                Deadline = task.Deadline,
                Status = Status.Active,
                Priority = task.Priority,
                IsChecked = false,
                CreateTime = DateTime.Now.ToUniversalTime(),
                UpdateTime = null
            };

            HandleUpdate(ref newTask);

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

        public async Task<TaskModel> GetTaskInfo(Guid id)
        {
            var taskEnt = await _taskRepository.GetTaskById(id);

            return new TaskModel
            {
                Id = taskEnt.Id,
                Name = taskEnt.Name,
                Description = taskEnt.Description,
                Deadline = taskEnt.Deadline,
                Priority = taskEnt.Priority,
                Status = taskEnt.Status,
                IsChecked = taskEnt.IsChecked,
                CreateTime = taskEnt.CreateTime,
                UpdateTime = taskEnt.UpdateTime
            };
        }

        public async Task CheckTask(Guid id)
        {
            var task = await _taskRepository.GetTaskById(id);

            if (task.IsChecked) throw new InvalidActionException(ErrorMessages.TASK_IS_ALREADY_COMPLETED);

            task.IsChecked = true;
            UpdateTaskStatus(ref task);

            task.UpdateTime = DateTime.Now.ToUniversalTime();

            await _taskRepository.SaveChanges();
        }

        public async Task UncheckTask(Guid id)
        {
            var task = await _taskRepository.GetTaskById(id);

            if (!task.IsChecked) throw new InvalidActionException(ErrorMessages.TASK_IS_ALREADY_UNCOMPLETED);

            task.IsChecked = false;
            UpdateTaskStatus(ref task);

            task.UpdateTime = DateTime.Now.ToUniversalTime();

            await _taskRepository.SaveChanges();
        }

        public async Task DeleteTask(Guid id)
        {
            await _taskRepository.DeleteTask(id);
        }

        public async Task EditTask(Guid id, TaskEditModel editedTask)
        {
            var task = await _taskRepository.GetTaskById(id);

            task.Name = editedTask.Name.Trim();
            task.Description = editedTask.Description.Trim();
            task.Deadline = editedTask.Deadline;
            task.Priority = editedTask.Priority;

            HandleUpdate(ref task);

            task.UpdateTime = DateTime.Now.ToUniversalTime();

            await _taskRepository.SaveChanges();
        }

        private void HandleUpdate(ref TaskEntity task)
        {
            UpdateTaskStatus(ref task);
            CheckPriority(ref task);
            CheckDeadline(ref task);
        }

        private void UpdateTaskStatus(ref TaskEntity task)
        {
            if (!task.IsChecked)
            {
                if (task.Deadline >= DateTime.Now.ToUniversalTime()) task.Status = Status.Active;
                else task.Status = Status.Overdue;
            }
            else
            {
                if (task.Deadline >= DateTime.Now.ToUniversalTime()) task.Status = Status.Completed;
                else task.Status = Status.Late;
            }

        }

        public void CheckPriority(ref TaskEntity task)
        {
            if (task.Name.Contains("!1"))
            {
                task.Name = task.Name.Replace("!1", "");
                task.Priority = Priority.Critical;
            }
                
            else if (task.Name.Contains("!2"))
            {
                task.Name = task.Name.Replace("!2", "");
                task.Priority = Priority.High;
            }

            else if (task.Name.Contains("!3"))
            {
                task.Name = task.Name.Replace("!3", "");
                task.Priority = Priority.Medium;
            }

            else if (task.Name.Contains("!4"))
            {
                task.Name = task.Name.Replace("!4", "");
                task.Priority = Priority.Low;
            }

            if (task.Name.Length < 4) throw new InvalidActionException(ErrorMessages.INVALID_LENGTH);
                
        }

        public void CheckDeadline(ref TaskEntity task)
        {
            Regex regex = new Regex("(!before \\d\\d\\.\\d\\d\\.\\d{4})|(!before \\d\\d\\-\\d\\d\\-\\d{4})");
            MatchCollection matches = regex.Matches(task.Name);

            foreach (Match match in matches)
            {
                string foundDate = match.Value.Split(" ")[1];
                DateTime date = DateTime.Now.ToUniversalTime();

                if (foundDate.Contains("-"))
                {
                    try
                    {
                        date = DateTime.ParseExact(foundDate,
                        "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidActionException(ErrorMessages.INVALID_DATE);
                    }
                }
                else
                {
                    try
                    {
                        date = DateTime.ParseExact(foundDate,
                        "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidActionException(ErrorMessages.INVALID_DATE);
                    }
                }

                task.Name = task.Name.Replace(match.Value, "");

                if (task.Name.Length < 4) throw new InvalidActionException(ErrorMessages.INVALID_LENGTH);

                task.Deadline = date.ToUniversalTime();
                return;
            }
        }
    }
}
