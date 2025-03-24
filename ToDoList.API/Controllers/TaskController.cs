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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskInfo([FromRoute] Guid id)
        {
            return Ok(await _taskService.GetTaskInfo(id));
        }

        [HttpPut("checked/{id}")]
        public async Task<IActionResult> CheckTask([FromRoute] Guid id)
        {
            await _taskService.CheckTask(id);

            return Ok();
        }

        [HttpDelete("checked/{id}")]
        public async Task<IActionResult> UncheckTask([FromRoute] Guid id)
        {
            await _taskService.UncheckTask(id);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            await _taskService.DeleteTask(id);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTask([FromRoute] Guid id, [FromBody] TaskEditModel task)
        {
            await _taskService.EditTask(id, task);

            return Ok();
        }
    }
}
