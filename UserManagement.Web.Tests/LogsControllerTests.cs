using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models.Logs;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Web.Tests;

public class LogsControllerTests
{
    [Fact]
    public async void List_WhenServiceReturnsLogs_ModelMustContainLogs()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var logs = new List<Log>
        {
            new Log
            {
                Id = 1,
                LogLevel = LogLevel.Information,
                EventId = 1001,
                UserAction = UserActions.FetchAllUsers,
                Message = "Fetched all users successfully.",
                Timestamp = DateTime.UtcNow,
                UserId = 1,
            },
        };

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Forename = "John",
                Surname = "Doe",
                Email = "email@mail.com",
            },
        };

        SetupLogs(logs, users);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        var logsOutput = logs.Select(l => new LogViewModel
            {
                Id = l.Id,
                LogLevel = l.LogLevel,
                EventId = l.EventId,
                UserAction = l.UserAction,
                Message = l.Message,
                Timestamp = l.Timestamp,
                User = users[0],
            })
            .ToList();

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult
            .Model.Should()
            .BeOfType<LogListViewModel>()
            .Which.Logs.Should()
            .BeEquivalentTo(logsOutput);
    }

    private List<Log> SetupLogs(List<Log> logs, List<User> users)
    {
        _logService.Setup(s => s.GetAll()).ReturnsAsync(logs);

        _userService
            .Setup(s => s.GetUser(It.IsAny<long>()))
            .ReturnsAsync((long id) => users.FirstOrDefault(u => u.Id == id));

        return logs;
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<ILogService> _logService = new();
    private readonly Mock<ILogger<LogsController>> _logger = new();

    private LogsController CreateController() =>
        new(_userService.Object, _logService.Object, _logger.Object);
}
