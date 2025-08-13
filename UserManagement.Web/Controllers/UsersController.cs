using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    private readonly ILogger _logger;

    public UsersController(
        IUserService userService,
        ILogService logService,
        ILogger<UsersController> logger
    )
    {
        _userService = userService;
        _logService = logService;
        _logger = logger;
    }

    [HttpGet]
    [Route("list")]
    public async Task<ViewResult> List(Boolean? active)
    {
        _logger.LogInformation(
            (int)UserActions.FetchAllUsers,
            "Retrieving user list with active filter: {Active}",
            active
        );

        var awaitItems =
            active != null ? _userService.FilterByActive((bool)active) : _userService.GetAll();
        var items = await awaitItems;

        var results = items.Select(p => new UserViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth.ToShortDateString(),
        });

        var model = new UserListViewModel { Items = results.ToList() };

        _logger.LogInformation(
            (int)UserActions.FetchAllUsers,
            "User list retrieved with {Count} items.",
            model.Items.Count
        );

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

        try
        {
            ValidateUser(addUser);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Validation failed for AddUserViewModel.");
            ModelState.AddModelError("DateOfBirth", e.Message);
            return View("AddUser", addUser);
        }

        var user = new User
        {
            Forename = addUser.Forename,
            Surname = addUser.Surname,
            Email = addUser.Email,
            IsActive = true,
            DateOfBirth = addUser.DateOfBirth,
        };

        _logger.LogInformation(
            "Adding new user: {Forename} {Surname}",
            user.Forename,
            user.Surname
        );
        var createdUser = await _userService.AddUser(user);

        LogWithUserScope(
            createdUser.Id,
            LogLevel.Information,
            UserActions.AddUser,
            "User created with ID {Id}",
            createdUser.Id
        );

        return RedirectToAction("ViewUser", new { id = createdUser.Id });
    }

    [HttpGet]
    [Route("User/{id}")]
    public async Task<ViewResult> ViewUser(long id)
    {
        LogWithUserScope(
            id,
            LogLevel.Information,
            UserActions.ViewUser,
            "Viewing user with ID {Id}",
            id
        );

        var user = await getUserViewModel(id);
        var logs = await _logService.GetLogsByUser(id);

        user.logs = logs.Select(log => new UserViewLogModel
            {
                LogLevel = log.LogLevel,
                Message = log.Message,
                Timestamp = log.Timestamp,
            })
            .ToList();

        return View(user);
    }

    [HttpGet]
    [Route("EditUser/{id}")]
    public async Task<IActionResult> EditUser(long id)
    {
        LogWithUserScope(
            id,
            LogLevel.Information,
            UserActions.EditUser,
            "Editing user with ID {Id}",
            id
        );
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
            editUser.Id = id;
            return View("EditUser", editUser);
        }

        try
        {
            ValidateUser(editUser);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Validation failed for AddUserViewModel.");
            ModelState.AddModelError("DateOfBirth", e.Message);
            editUser.Id = id;
            return View("EditUser", editUser);
        }

        LogWithUserScope(
            id,
            LogLevel.Information,
            UserActions.FetchUser,
            "Retrieving user with ID {Id}",
            id
        );
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
        return RedirectToAction("ViewUser", new { id });
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
            _logger.LogError("User with ID {Id} not found for deletion.", id);
            return NotFound($"User with ID {id} not found.");
        }

        LogWithUserScope(
            user.Id,
            LogLevel.Information,
            UserActions.DeleteUser,
            "Deleting user with ID {Id}",
            user.Id
        );
        await _userService.DeleteUser(user);
        return RedirectToAction("list");
    }

    private async Task<UserViewModel> getUserViewModel(long id)
    {
        LogWithUserScope(
            id,
            LogLevel.Information,
            UserActions.FetchUser,
            "Retrieving user with ID {Id}",
            id
        );
        var user = await _userService.GetUser(id);

        if (user == null)
        {
            _logger.LogError("User with ID {Id} not found.", id);
            throw new Exception($"User with ID {id} not found.");
        }

        return UserViewModel.FromUser(user);
    }

    private void LogWithUserScope(
        long userId,
        LogLevel level,
        UserActions logEvent,
        string message,
        params object[] args
    )
    {
        using (
            _logger.BeginScope(
                new Dictionary<string, object> { ["UserId"] = userId, ["UserAction"] = logEvent }
            )
        )
        {
            _logger.Log(level, message, args);
        }
    }

    private void ValidateUser(AddUserViewModel user)
    {
        var errors = new List<Exception>();
        if (user.DateOfBirth > DateTime.UtcNow.AddYears(-18))
        {
            errors.Add(new ArgumentException("User must be at least 18 years old"));
        }
        if (!Regex.IsMatch(user.Forename, @"^[a-zA-Z\s\-']+$"))
        {
            errors.Add(new ArgumentException("Forename", "Forename contains invalid characters."));
        }
        if (!Regex.IsMatch(user.Surname, @"^[a-zA-Z\s\-']+$"))
        {
            errors.Add(new ArgumentException("Surname", "Surname contains invalid characters."));
        }

        if (errors.Count > 0)
        {
            throw new AggregateException("User validation failed", errors);
        }
        // TODO: check for unique email address
    }
}
