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
    public class CheckDeadlineTests
    {
        private readonly TaskServiceImpl _taskService;

        public CheckDeadlineTests()
        {
            var repositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskServiceImpl(repositoryMock.Object);
        }

        public static IEnumerable<object[]> CorrectDeadlineTestCases => new List<object[]>
        {
            new object[] { "Test task !before 10.10.2020", 
                new DateTime(2020, 10, 10).ToUniversalTime(), "Test task " },
            new object[] { "Test task !before 10-10-2020", 
                new DateTime(2020, 10, 10).ToUniversalTime(), "Test task " },
            new object[] { "Min Date !before 01.01.0001", 
                new DateTime(0001, 01, 01).ToUniversalTime(), "Min Date " },
            new object[] { "Max Date !before 31.12.9999", 
                new DateTime(9999, 12, 31).ToUniversalTime(), "Max Date " },
        };

        public static IEnumerable<object[]> InvalidDateTestCases => new List<object[]>
        {
            new object[] { "Test task !before 10.13.2020" },
            new object[] { "Test task !before 34-10-2020" },
            new object[] { "Test task !before 10-10-0000" }
        };

        public static IEnumerable<object[]> InvalidMacrosTestCases => new List<object[]>
        {
            new object[] { "Test task !before 10/10.2020" },
            new object[] { "Test task !before 10.10-2020" },
            new object[] { "Test task !before 10/10/2020" },
            new object[] { "Test task !before10/10/2020" }
        };

        [Theory]
        [MemberData(nameof(CorrectDeadlineTestCases))]
        public void CheckDeadline_WithMacros_SetExpectedDeadline(
            string taskName,
            DateTime expectedDeadline,
            string expectedName)
        {
            // arrange
            var task = new TaskEntity { Name = taskName };

            // act
            _taskService.CheckDeadline(ref task);

            // assert
            Assert.Equal(expectedDeadline, task.Deadline);
            Assert.Equal(expectedName, task.Name);
        }

        [Theory]
        [MemberData(nameof(InvalidMacrosTestCases))]
        public void CheckDeadline_WithInvalidMacros_NoChanges(
            string taskName)
        {
            var task = new TaskEntity { Name = taskName };

            _taskService.CheckDeadline(ref task);

            Assert.Equal(taskName, task.Name);
            Assert.Null(task.Deadline);
        }

        [Fact]
        public void CheckDeadline_ShortName_ThrowsException()
        {
            var task = new TaskEntity { Name = "a !before 10-10-2020" };

            var exception = Assert.Throws<InvalidActionException>(() => _taskService.CheckDeadline(ref task));

            Assert.Equal(ErrorMessages.INVALID_LENGTH, exception.Message);
        }

        [Theory]
        [MemberData(nameof(InvalidDateTestCases))]
        public void CheckDeadline_InvalidDate_ThrowsException(string taskName)
        {
            var task = new TaskEntity { Name = taskName };

            var exception = Assert.Throws<InvalidActionException>(() => _taskService.CheckDeadline(ref task));

            Assert.Equal(ErrorMessages.INVALID_DATE, exception.Message);
        }
    }
}