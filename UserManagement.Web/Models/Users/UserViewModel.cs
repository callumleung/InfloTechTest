using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Web.Models.Users;

public class UserViewModel
{
    public long Id { get; set; }
    public string? Forename { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public string? DateOfBirth { get; set; }

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

    // Arguably UserViewModel and AddUserViewModel should just be a single model
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
