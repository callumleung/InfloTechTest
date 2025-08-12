using System.Linq;
using FluentAssertions;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public async void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        await context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = await context.GetAll<User>();
        await context.Delete(entity.First());

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.First().Email);
    }

    [Fact]
    public async void GetActiveUsers_WhenCalledWithTrue_MustReturnOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetActiveUsers(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().OnlyContain(u => u.IsActive);
    }

    [Fact]
    public async void GetActiveUsers_WhenCalledWithFalse_MustInactiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = new User
        {
            Forename = "inactive",
            Surname = "User",
            Email = "notActive@example.com",
            IsActive = false
        };
        await context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetActiveUsers(false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().OnlyContain(u => !u.IsActive);
    }

    private DataContext CreateContext() => new();
}
