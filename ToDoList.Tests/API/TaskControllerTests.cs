using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Xml.Linq;
using ToDoList.Common.Constants;
using ToDoList.Common.Exceptions;
using ToDoList.Common.Models;
using ToDoList.Persistence.Entities;
using FluentAssertions;
using ToDoList.Common.Enums;
using Microsoft.AspNetCore.Http;


namespace ToDoList.Tests.API
{
    public class TaskControllerTests : IClassFixture<IntegrationWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TaskControllerTests(IntegrationWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<Guid> CreateTask(string name)
        {
            var task = new TaskCreateModel
            {
                Name = name
            };

            var response = await _client.PostAsJsonAsync("/api/task", task);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IdResponseModel>();

            return result!.Id;
        }

        [Fact]
        public async Task CreateTask_ReturnsOk_WithValidInput()
        {
            var task = new TaskEntity
            {
                Name = "First Task",
                Description = "description",
                Deadline = DateTime.UtcNow.ToUniversalTime()
            };

            var response = await _client.PostAsJsonAsync("/api/task", task);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<IdResponseModel>();
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTaskInfo_ReturnsCorrectData()
        {
            var id = await CreateTask("Created Task");
            
            var response = await _client.GetAsync($"/api/task/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var task = await response.Content.ReadFromJsonAsync<TaskModel>();
            task!.Id.Should().Be(id);
            task!.Name.Should().Be("Created Task");
        }

        [Fact]
        public async Task GetAllTasks_ReturnsList_SortedByPriority()
        {
            await CreateTask("First task !1");
            await CreateTask("Second task !4");

            var response = await _client.GetAsync("/api/task/all?filter=PriorityAsc&isChecked=Both");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var list = await response.Content.ReadFromJsonAsync<TaskListModel>();
            list!.Tasks.Should().HaveCountGreaterThan(1);
            list!.Tasks[0].Name.Should().Be("Second task");
        }

        [Fact]
        public async Task Check_Uncheck_Task()
        {
            var id = await CreateTask("Check uncheck task");

            await _client.PutAsync($"/api/task/checked/{id}", null);
            var checkedTask = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            checkedTask!.IsChecked.Should().BeTrue();

            await _client.DeleteAsync($"/api/task/checked/{id}");
            var uncheckedTask = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            uncheckedTask!.IsChecked.Should().BeFalse();
        }

        [Fact]
        public async Task EditTask_ChangesValues()
        {
            var id = await CreateTask("Edit task");

            var newDeadline = DateTime.UtcNow.AddDays(2).ToUniversalTime();
            var editModel = new TaskEditModel
            {
                Name = "Edit task Updated",
                Deadline = newDeadline
            };
            await _client.PutAsJsonAsync($"/api/task/{id}", editModel);

            var result = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            result!.Name.Should().Be("Edit task Updated");
            result!.Deadline.Should().Be(newDeadline);
        }

        [Fact]
        public async Task DeleteTask_RemovesFromList()
        {
            var id = await CreateTask("Task to delete");

            await _client.DeleteAsync($"/api/task/{id}");

            var response = await _client.GetAsync($"/api/task/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            response = await _client.GetAsync("/api/task/all");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var list = await response.Content.ReadFromJsonAsync<TaskListModel>();
            list!.Tasks.Should().NotContain(task => task.Id == id);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("aaa", false)]
        [InlineData("aaaa", true)]
        [InlineData("Normal task", true)]
        public async Task CreateTask_NameValidation(string name, bool isValid)
        {
            var task = new TaskCreateModel
            {
                Name = name
            };

            var response = await _client.PostAsJsonAsync("/api/task", task);

            if (isValid)
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            else
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("1", 400)]
        [InlineData("00000000-0000-0000-0000-000000000000", 404)]
        public async Task GetTaskInfo_InvalidId(string id, int statusCode)
        {
            var response = await _client.GetAsync($"/api/task/{id}");
            response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Theory]
        [InlineData("1", 400)]
        [InlineData("00000000-0000-0000-0000-000000000000", 404)]
        public async Task DeleteTask_InvalidId(string id, int statusCode)
        {
            var response = await _client.DeleteAsync($"/api/task/{id}");
            response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Theory]
        [InlineData("1", 400, true)]
        [InlineData("00000000-0000-0000-0000-000000000000", 404, true)]
        [InlineData("1", 400, false)]
        [InlineData("00000000-0000-0000-0000-000000000000", 404, false)]
        public async Task CheckTask_InvalidId(string id, int statusCode, bool check)
        {
            HttpResponseMessage? response;
            if (check)
                response = await _client.PutAsync($"/api/task/checked/{id}", null);
            else
                response = await _client.DeleteAsync($"/api/task/checked/{id}");

            response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Theory]
        [InlineData("1", 400)]
        [InlineData("00000000-0000-0000-0000-000000000000", 404)]
        public async Task EditTask_InvalidId(string id, int statusCode)
        {
            var editModel = new TaskEditModel
            {
                Name = "Edit task Updated",
                Deadline = DateTime.Now.ToUniversalTime()
            };
            var response = await _client.PutAsJsonAsync($"/api/task/{id}", editModel);

            response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Fact]
        public async Task Check_TwoTimes_Task()
        {
            var id = await CreateTask("Check task two times");

            await _client.PutAsync($"/api/task/checked/{id}", null);
            var checkedTask = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            checkedTask!.IsChecked.Should().BeTrue();

            var response = await _client.PutAsync($"/api/task/checked/{id}", null);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Uncheck_TwoTimes_Task()
        {
            var id = await CreateTask("Uncheck task two times");

            await _client.PutAsync($"/api/task/checked/{id}", null);
            var checkedTask = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            checkedTask!.IsChecked.Should().BeTrue();

            await _client.DeleteAsync($"/api/task/checked/{id}");
            var uncheckedTask = await _client.GetFromJsonAsync<TaskModel>($"/api/task/{id}");
            uncheckedTask!.IsChecked.Should().BeFalse();

            var response = await _client.DeleteAsync($"/api/task/checked/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
