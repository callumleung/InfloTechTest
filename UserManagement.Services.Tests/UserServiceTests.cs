using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public async void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public async void GetUser_WhenContextReturnsEntity_MustReturnSameEntity()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = await service.GetUser(3);

        // Assert
        result.Should().BeSameAs(users.Last());
    }

    [Fact]
    public async void AddUser_WhenCalled_ShouldAddUserToContext()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();
        var newUser = new User
        {
            Forename = "John",
            Surname = "Doe",
            Email = "jd@gmail.com",
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        _dataContext
           .Setup(s => s.Create<User>(It.IsAny<User>()))
           .Callback<User>(u => users = users.Append(newUser));

        // Act
        await service.AddUser(newUser);

        // Assert
        users.Count().Should().Be(4);
        users.Contains(newUser).Should().BeTrue();
    }

    [Fact]
    public async void UpdateUser_WhenCalled_ShouldUpdateUserInContext()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();
        var userToUpdate = users.First(u => u.Id == 1);
        userToUpdate.Forename = "Updated Name";

        // Act
        await service.UpdateUser(userToUpdate);

        // Assert
        users.First(u => u.Id == 1).Forename.Should().Be("Updated Name");
    }


    private IQueryable<User> SetupUsers()
    {
        var users = new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = new DateTime(1990, 1, 1) },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = new DateTime(1990, 1, 1) },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = new DateTime(1990, 1, 1) },
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .ReturnsAsync(users);

        _dataContext
            .Setup(s => s.Create<User>(It.IsAny<User>()))
            .Callback<User>(u => users = users.Append(u));

        _dataContext
            .Setup(s => s.Update<User>(It.IsAny<User>()))
            .Callback<User>(u => users = users.Select(x => x.Id == u.Id ? u : x));

        _dataContext
            .Setup(s => s.GetById<User>(It.IsAny<long>()))
            .ReturnsAsync((long id) => users.FirstOrDefault(u => u.Id == id));


        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
