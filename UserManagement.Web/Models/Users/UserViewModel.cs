using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Models.Users;

public class UserViewModel
{
    public long Id { get; set; }
    public string? Forename { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public string? DateOfBirth { get; set; }
    public List<UserViewLogModel> logs { get; set; } = new();

    public static UserViewModel FromUser(User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth.ToShortDateString()
        };
    }

    public static UserViewModel FromAddUserView(AddUserViewModel user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth.ToShortDateString()
        };
    }
}

public class UserViewLogModel
{
    public string Message { get; set; } = default!;

    public LogLevel? LogLevel { get; set; }

    public DateTime Timestamp { get; set; }
}
