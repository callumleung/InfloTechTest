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

        var results = items.Select(p => new UserViewModel
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
    public IActionResult AddUser(AddUserViewModel addUser)
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
        return RedirectToAction("list");
    }

    [HttpGet]
    [Route("User")]
    public ViewResult ViewUser(long userId)
    {
        return View(getUserViewModel(userId));
    }

    [HttpGet]
    [Route("EditUserView")]
    public ViewResult EditUser(long userId)
    {
        var user = getUserViewModel(userId);
        return View("EditUser", user);
    }

    [HttpPost]
    [Route("EditUser")]
    public IActionResult EditUser(AddUserViewModel userViewModel, long id)
    {
        var user = _userService.GetUser(id);

        user.Forename = userViewModel.Forename;
        user.Surname = userViewModel.Surname;
        user.Email = userViewModel.Email;
        user.DateOfBirth = userViewModel.DateOfBirth;
        user.IsActive = userViewModel.IsActive;

        _userService.UpdateUser(user);
        return RedirectToAction("list");
    }


    private UserViewModel getUserViewModel(long userId)
    {
        var user = _userService.GetUser(userId);
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
