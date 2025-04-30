using Moq;
using ToDoList.Application;
using ToDoList.Application.Implementations;
using ToDoList.Common.Constants;
using ToDoList.Common.Enums;
using ToDoList.Common.Exceptions;
using ToDoList.Persistence.Entities;
using ToDoList.Persistence.Repositories;

namespace ToDoList.Tests
{
    public class CheckPriorityTests
    {
        private readonly TaskServiceImpl _taskService;

        public CheckPriorityTests()
        {
            var repositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskServiceImpl(repositoryMock.Object);
        }

        public static IEnumerable<object[]> CorrectPriorityTestCases => new List<object[]>
        {
            new object[] { "Critical priority !1", Priority.Critical, "Critical priority " },
            new object[] { "High priority !2", Priority.High, "High priority " },
            new object[] { "Medium priority !3", Priority.Medium, "Medium priority " },
            new object[] { "Low priority !4", Priority.Low, "Low priority " }
        };
        public static IEnumerable<object[]> InvalidPriorityMacrosTestCases => new List<object[]>
        {
            new object[] { "Normal task !5" },
            new object[] { "Normal task !0" }
        };

        [Theory]
        [MemberData(nameof(CorrectPriorityTestCases))]
        public void CheckPriority_WithMacros_SetExpectedPriority(
            string taskName,
            Priority expectedPriority,
            string expectedName)
        {
            // arrange
            var task = new TaskEntity { Name = taskName };

            // act
            _taskService.CheckPriority(ref task);

            // assert
            Assert.Equal(expectedPriority, task.Priority);
            Assert.Equal(expectedName, task.Name);
        }

        [Fact]
        public void CheckPriority_ShortName_ThrowsException()
        {
            var task = new TaskEntity { Name = "!1" };

            var exception = Assert.Throws<InvalidActionException>(() => _taskService.CheckPriority(ref task));

            Assert.Equal(ErrorMessages.INVALID_LENGTH, exception.Message);
        }

        [Fact]
        public void CheckPriority_MinimalAllowedLength_SetPriority()
        {
            var task = new TaskEntity { Name = "test!1" };

            _taskService.CheckPriority(ref task);

            Assert.Equal(Priority.Critical, task.Priority);
        }

        [Fact]
        public void CheckPriority_MaximalUnallowedLength_ThrowsException()
        {
            var task = new TaskEntity { Name = "abc!1" };

            var exception = Assert.Throws<InvalidActionException>(() => _taskService.CheckPriority(ref task));

            Assert.Equal(ErrorMessages.INVALID_LENGTH, exception.Message);
        }

        [Fact]
        public void CheckPriority_NoMacros_NoChanges()
        {
            var task = new TaskEntity { Name = "Normal task" };

            _taskService.CheckPriority(ref task);

            Assert.Equal("Normal task", task.Name);
        }

        [Theory]
        [MemberData(nameof(InvalidPriorityMacrosTestCases))]
        public void CheckPriority_InvalidMacros_NoChanges(string taskName)
        {
            var task = new TaskEntity { Name = taskName };

            _taskService.CheckPriority(ref task);

            Assert.Equal(Priority.Medium, task.Priority);
            Assert.Equal(taskName, task.Name);
        }
    }
}