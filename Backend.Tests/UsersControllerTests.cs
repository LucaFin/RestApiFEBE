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
    public class UsersControllerTests
    {
        private readonly Mock<IJsonPlaceholderService> _mockService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockService = new Mock<IJsonPlaceholderService>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            _controller = new UsersController(_mockService.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsExpectedUsers()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetUsersAsync())
                    .ReturnsAsync(GetTestUsers());

                // Act
                var result = await _controller.GetUsers(10, 0);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<User>>(okResult.Value);
                Assert.Equal(3, returnValue.Items.Count());

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("GetUsers called with limit"));
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        [Fact]
        public async Task GetUsers_NoUsers_ReturnsEmptyList()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetUsersAsync())
                    .ReturnsAsync(new List<User>());

                // Act
                var result = await _controller.GetUsers(10, 0);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<User>>(okResult.Value);
                Assert.Empty(returnValue.Items);

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        [Fact]
        public async Task GetUsers_Pagination_ReturnsCorrectUsers()
        {
            using (TestCorrelator.CreateContext())
            {
                // Arrange
                _mockService.Setup(service => service.GetUsersAsync())
                    .ReturnsAsync(GetTestUsers());

                // Act
                var result = await _controller.GetUsers(2, 1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsAssignableFrom<PaginatedResponse<User>>(okResult.Value);
                Assert.Equal(2, returnValue.Items.Count());
                Assert.Equal(2, returnValue.Items.First().Id); // Ensure correct pagination

                // Verify logs
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.Contains(logEvents, e => e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Returning"));
            }
        }

        private List<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "User 1" },
                new User { Id = 2, Name = "User 2" },
                new User { Id = 3, Name = "User 3" }
            };
        }
    }
}
