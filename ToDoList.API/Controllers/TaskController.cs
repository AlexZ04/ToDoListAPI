using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Application;
using ToDoList.Common.Enums;
using ToDoList.Common.Models;

namespace ToDoList.API.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        { 
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel task)
        {
            return Ok(await _taskService.CreateTask(task));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTasks([FromQuery] TaskFilter filter, [FromQuery] IsChecked isChecked)
        {
            return Ok(await _taskService.GetAllTasks(filter, isChecked));
        }
    }
}
