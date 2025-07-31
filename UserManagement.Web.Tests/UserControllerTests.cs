using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(null);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive,
            DateOfBirth = u.DateOfBirth.ToShortDateString()
        }).ToList();
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(usersOutput);
    }

    [Fact]
    public void List_WhenServiceReturnsActiveUsers_ModelMustContainOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive,
            DateOfBirth = u.DateOfBirth.ToShortDateString()
        }).ToList();
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(usersOutput);
    }

    [Fact]
    public void List_WhenServiceReturnsNonActiveUsers_ModelMustContainOnlyNonActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers("Johnny", "User", "juser@example.com", false, default);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive,
            DateOfBirth = u.DateOfBirth.ToShortDateString()
        }).ToList();
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(usersOutput);
    }

    [Fact]
    public void AddUser_WhenCalled_shouldConstructUserAndCallService()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User",
            Email = "test@email.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.AddUser(It.Is<User>(u =>
            u.Forename == addUser.Forename &&
            u.Surname == addUser.Surname &&
            u.Email == addUser.Email &&
            u.IsActive &&
            u.DateOfBirth == addUser.DateOfBirth)), Times.Once);

    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true, DateTime dateOfBirth = default)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = dateOfBirth
            }
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        _userService
            .Setup(s => s.FilterByActive(It.IsAny<bool>()))
            .Returns(users);

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
