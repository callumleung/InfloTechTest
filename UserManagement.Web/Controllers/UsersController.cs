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
        IEnumerable<User> items = active != null ? _userService.FilterByActive((bool)active) : _userService.GetAll();

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
    [Route("AddUserView")]
    public ViewResult AddUser()
    {
        return View("AddUser");
    }

    [HttpPost]
    [Route("AddUserViewModel")]
    public IActionResult AddUser(AddUserViewModel addUser)
    {
        if (!ModelState.IsValid)
        {
            // TODO: preserve submitted data in the view model (template is currently not reading from model)
            return View("AddUser", addUser);
        }

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
    [Route("User/{id}")]
    public ViewResult ViewUser(long id)
    {
        return View(getUserViewModel(id));
    }

    [HttpGet]
    [Route("EditUser/{id}")]
    public ViewResult EditUser(long id)
    {
        var user = getUserViewModel(id);
        return View("EditUser", user);
    }

    [HttpPost]
    [Route("EditUser/{id}")]
    public IActionResult EditUser(long id, AddUserViewModel editUser)
    {
        if (!ModelState.IsValid)
        {
            var returnUser = UserViewModel.FromAddUserView(editUser);
            returnUser.Id = id;
            return View("EditUser",returnUser);
        }

        var user = _userService.GetUser(id);
        user.Forename = editUser.Forename;
        user.Surname = editUser.Surname;
        user.Email = editUser.Email;
        user.DateOfBirth = editUser.DateOfBirth;
        user.IsActive = editUser.IsActive;

        _userService.UpdateUser(user);
        return RedirectToAction("list");
    }


    [HttpGet]
    [Route("DeleteUser/{id}")]
    public IActionResult getDeleteUser(long id)
    {
        var user = getUserViewModel(id);
        return View("DeleteUser", user);
    }


    [HttpPost]
    [Route("DeleteUser/{id}")]
    public IActionResult DeleteUser(long id)
    {
        var user = _userService.GetUser(id);
        _userService.DeleteUser(user);
        return RedirectToAction("list");
    }

    private UserViewModel getUserViewModel(long id)
    {
        var user = _userService.GetUser(id);
        return UserViewModel.FromUser(user);
    }
}
