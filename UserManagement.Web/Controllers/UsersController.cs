using System;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task<ViewResult> List(Boolean? active)
    {
        var awaitItems = active != null ? _userService.FilterByActive((bool)active) : _userService.GetAll();
        var items = await awaitItems;

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
    public async Task<IActionResult> AddUser(AddUserViewModel addUser)
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

        await _userService.AddUser(user);
        return RedirectToAction("list");
    }

    [HttpGet]
    [Route("User/{id}")]
    public async Task<ViewResult> ViewUser(long id)
    {
        return View(await getUserViewModel(id));
    }

    [HttpGet]
    [Route("EditUser/{id}")]
    public async Task<IActionResult> EditUser(long id)
    {
        var user = await _userService.GetUser(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        var result = UserViewModel.FromUser(user);
        return View("EditUser", result);
    }

    [HttpPost]
    [Route("EditUser/{id}")]
    public async Task<IActionResult> EditUser(long id, AddUserViewModel editUser)
    {
        if (!ModelState.IsValid)
        {
            var returnUser = UserViewModel.FromAddUserView(editUser);
            returnUser.Id = id;
            return View("EditUser", returnUser);
        }

        var user = await _userService.GetUser(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        user.Forename = editUser.Forename;
        user.Surname = editUser.Surname;
        user.Email = editUser.Email;
        user.DateOfBirth = editUser.DateOfBirth;
        user.IsActive = editUser.IsActive;

        await _userService.UpdateUser(user);
        return RedirectToAction("list");
    }


    [HttpGet]
    [Route("DeleteUser/{id}")]
    public async Task<IActionResult> getDeleteUser(long id)
    {
        var user = await getUserViewModel(id);
        return View("DeleteUser", user);
    }


    [HttpPost]
    [Route("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _userService.GetUser(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        await _userService.DeleteUser(user);
        return RedirectToAction("list");
    }

    private async Task<UserViewModel> getUserViewModel(long id)
    {
        var user = await _userService.GetUser(id);

        if (user == null)
        {
            throw new Exception($"User with ID {id} not found.");
        }

        return UserViewModel.FromUser(user);
    }
}
