using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public async void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(null);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                Forename = u.Forename,
                Surname = u.Surname,
                Email = u.Email,
                IsActive = u.IsActive,
                DateOfBirth = u.DateOfBirth.ToShortDateString(),
            })
            .ToList();

        result
            .Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should()
            .BeEquivalentTo(usersOutput);
    }

    [Fact]
    public async void List_WhenServiceReturnsActiveUsers_ModelMustContainOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                Forename = u.Forename,
                Surname = u.Surname,
                Email = u.Email,
                IsActive = u.IsActive,
                DateOfBirth = u.DateOfBirth.ToShortDateString(),
            })
            .ToList();

        result
            .Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should()
            .BeEquivalentTo(usersOutput);
    }

    [Fact]
    public async void List_WhenServiceReturnsNonActiveUsers_ModelMustContainOnlyNonActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers("Johnny", "User", "juser@example.com", false, default);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var usersOutput = users
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                Forename = u.Forename,
                Surname = u.Surname,
                Email = u.Email,
                IsActive = u.IsActive,
                DateOfBirth = u.DateOfBirth.ToShortDateString(),
            })
            .ToList();

        result
            .Model.Should()
            .BeOfType<UserListViewModel>()
            .Which.Items.Should()
            .BeEquivalentTo(usersOutput);
    }

    [Fact]
    public async void AddUser_WhenCalled_shouldConstructUserAndCallService()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        SetupUsers();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User",
            Email = "test@email.com",
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(
            s =>
                s.AddUser(
                    It.Is<User>(u =>
                        u.Forename == addUser.Forename
                        && u.Surname == addUser.Surname
                        && u.Email == addUser.Email
                        && u.IsActive
                        && u.DateOfBirth == addUser.DateOfBirth
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async void AddUser_WhenModelStateIsInvalid_shouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User",
            Email = "invalid-email@gmail.com", // Invalid email to trigger model state error
            DateOfBirth = new DateTime(1990, 1, 1),
        };
        controller.ModelState.AddModelError("Email", "Invalid email format");

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(addUser);
    }

    //TODO: Add tests for the AddUser/EditUser methods to cover all validation scenarios.

    [Fact]
    public async void AddUser_WhenUserAgeIsUnder18_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User",
            Email = "email@gmail.com",
            DateOfBirth = DateTime.Now,
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(addUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void AddUser_WhenUserForenameContainsNumbers_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny5555",
            Surname = "User",
            Email = "email@email.com", // Invalid email to trigger model state error
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(addUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void AddUser_WhenUserSurnameContainsNumbers_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var addUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User555",
            Email = "email@email.com", // Invalid email to trigger model state error
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddUser(addUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(addUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void EditUser_WhenUserAgeIsUnder18_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var editUser = new AddUserViewModel
        {
            Forename = "newName",
            Surname = "User",
            Email = "email@email.com", // Invalid email to trigger model state error
            DateOfBirth = DateTime.Now,
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.EditUser(0, editUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(editUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void EditUser_WhenUserForenameContainsNumbers_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var editUser = new AddUserViewModel
        {
            Forename = "Johnny555",
            Surname = "User",
            Email = "email@email.com", // Invalid email to trigger model state error
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.EditUser(0, editUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(editUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void EditUser_WhenUserSurnameContainsNumbers_ShouldReturnViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var editUser = new AddUserViewModel
        {
            Forename = "Johnny",
            Surname = "User555",
            Email = "email@email.com", // Invalid email to trigger model state error
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.EditUser(0, editUser);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(editUser);
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async void DeleteUser_WhenCalled_shouldCallService()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers()[0];

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.DeleteUser(user.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.GetUser(user.Id), Times.Once);
        _userService.Verify(s => s.DeleteUser(It.Is<User>(u => u.Id == user.Id)), Times.Once);
    }

    private List<User> SetupUsers(
        string forename = "Johnny",
        string surname = "User",
        string email = "juser@example.com",
        bool isActive = true,
        DateTime dateOfBirth = default
    )
    {
        var users = new List<User>
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = dateOfBirth,
            },
        };

        _userService.Setup(s => s.GetAll()).ReturnsAsync(users);

        _userService.Setup(s => s.FilterByActive(It.IsAny<bool>())).ReturnsAsync(users);

        _userService
            .Setup(s => s.DeleteUser(It.IsAny<User>()))
            .Callback<User>(u => users.RemoveAll(x => x.Id == u.Id));

        _userService
            .Setup(s => s.GetUser(It.IsAny<long>()))
            .ReturnsAsync((long id) => users.FirstOrDefault(u => u.Id == id));

        _userService
            .Setup(s => s.AddUser(It.IsAny<User>()))
            .ReturnsAsync(
                (User user) =>
                {
                    user.Id = 20;
                    return user;
                }
            );

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<ILogService> _logService = new();
    private readonly Mock<ILogger<UsersController>> _logger = new();

    private UsersController CreateController() =>
        new(_userService.Object, _logService.Object, _logger.Object);
}
