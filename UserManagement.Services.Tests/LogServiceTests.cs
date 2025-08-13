using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;

namespace UserManagement.Data.Tests;

public class LogServiceTests
{
    [Fact]
    public async void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var logs = SetupLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async void GetLogsByUser_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService();
        var logs = SetupLogs();

        // Act
        var result = await service.GetLogsByUser(1);

        // Assert
        result.Should().BeEquivalentTo(logs.Where(l => l.UserId == 1));
    }

    private IQueryable<Log> SetupLogs()
    {
        var logs = new List<Log>
        {
            new Log
            {
                Id = 1,
                Message = "Log 1",
                UserId = 1,
            },
            new Log { Id = 2, Message = "Log 2" },
        };

        _dataContext.Setup(s => s.GetAll<Log>()).ReturnsAsync(logs);

        _dataContext
            .Setup(s => s.GetLogsForUser(It.IsAny<long>()))
            .ReturnsAsync((long userId) => logs.Where(log => log.UserId == userId).ToList());

        return logs.AsQueryable();
    }

    private readonly Mock<IDataContext> _dataContext = new();

    private LogService CreateService() => new(_dataContext.Object);
}
