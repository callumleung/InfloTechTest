using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [Route("list")]
    public ViewResult List(Boolean? active)
    {
        IEnumerable<User> items = active != null ? _userService.FilterByActive((bool) active) : _userService.GetAll();

        var results = items.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth.ToShortDateString()
        });

        var model = new UserListViewModel
        {
            Items = results.ToList()
        };

        return View(model);

    }

    [HttpGet]
    [Route("AddUser")]
    public ViewResult GetAddUser()
    {
        return View("AddUser");
    }

    [HttpPost]
    [Route("AddUserViewModel")]
    public ViewResult AddUser(AddUserViewModel addUser)
    {
        var user = new User
        {
            Forename = addUser.Forename,
            Surname = addUser.Surname,
            Email = addUser.Email,
            IsActive = true,
            DateOfBirth = addUser.DateOfBirth
        };

        _userService.AddUser(user);
        return View(new AddUserViewModel());
    }

    [HttpGet]
    [Route("User")]
    public ViewResult ViewUser(long userId)
    {
        var user = _userService.GetUser(userId);
        var result = user.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth.ToShortDateString()
        }).FirstOrDefault(new UserListItemViewModel());

        return View(result);
    }
}
