using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class TodosControllerTests
    {
        private readonly Mock<IJsonPlaceholderService> _mockService;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockService = new Mock<IJsonPlaceholderService>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            _controller = new TodosController(_mockService.Object);
        }

        [Fact]
        public async Task GetTodos_ReturnsExpectedTodos()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetTodosByUserIdAsync(1))
                    .ReturnsAsync(GetTestTodos().Where(t => t.UserId == 1).ToList());

                // Act
                var result = await _controller.GetTodos(1, 10, 0);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<Todo>>(okResult.Value);
                Assert.Equal(2, returnValue.Items.Count());

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("GetTodos called with userId"));
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        [Fact]
        public async Task GetTodos_InvalidLimit_ReturnsBadRequest()
        {
            using (TestCorrelator.CreateContext())
            {
                // Act
                var result = await _controller.GetTodos(1, -1, 0);

                // Assert
                Assert.IsType<BadRequestResult>(result);

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains("Invalid limit or offset"));
            }
        }

        [Fact]
        public async Task GetTodos_InvalidOffset_ReturnsBadRequest()
        {
            using (TestCorrelator.CreateContext())
            {
                // Act
                var result = await _controller.GetTodos(1, 10, -1);

                // Assert
                Assert.IsType<BadRequestResult>(result);

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains("Invalid limit or offset"));
            }
        }

        [Fact]
        public async Task GetTodos_ServiceError_ReturnsInternalServerError()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetTodosByUserIdAsync(1))
                    .ThrowsAsync(new System.Exception("Service error"));

                // Act
                var result = await _controller.GetTodos(1, 10, 0);

                // Assert
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal(500, objectResult.StatusCode);

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Error && e.MessageTemplate.Text.Contains("An error occurred while getting todos"));
            }
        }

        [Fact]
        public async Task GetTodos_NoTodos_ReturnsEmptyList()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetTodosByUserIdAsync(1))
                    .ReturnsAsync(new List<Todo>());

                // Act
                var result = await _controller.GetTodos(1, 10, 0);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<Todo>>(okResult.Value);
                Assert.Empty(returnValue.Items);

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        [Fact]
        public async Task GetTodos_Pagination_ReturnsCorrectTodos()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetTodosByUserIdAsync(1))
                    .ReturnsAsync(GetTestTodos().Where(t => t.UserId == 1).ToList());

                // Act
                var result = await _controller.GetTodos(1, 1, 1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<Todo>>(okResult.Value);
                Assert.Single(returnValue.Items);
                Assert.Equal(2, returnValue.Items.First().Id); // Ensure correct pagination

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        private List<Todo> GetTestTodos()
        {
            return new List<Todo>
            {
                new Todo { Id = 1, UserId = 1, Title = "Todo 1", Completed = false },
                new Todo { Id = 2, UserId = 1, Title = "Todo 2", Completed = true },
                new Todo { Id = 3, UserId = 2, Title = "Todo 3", Completed = false }
            };
        }
    }
}
