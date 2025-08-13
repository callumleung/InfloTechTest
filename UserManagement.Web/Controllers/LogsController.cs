using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Logs;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Web.Controllers;

[Route("Logs")]
public class LogsController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    private readonly ILogger _logger;

    public LogsController(
        IUserService userService,
        ILogService logService,
        ILogger<LogsController> logger
    )
    {
        _userService = userService;
        _logService = logService;
        _logger = logger;
    }

    [HttpGet]
    [Route("List")]
    public async Task<ViewResult> List()
    {
        _logger.LogInformation((int)LogActions.FetchAll, "Retrieving all Logs.");
        var logs = await _logService.GetAll();

        var results = await Task.WhenAll(
            logs.Select(async log =>
                {
                    User? user = null;
                    if (log.UserId != null)
                    {
                        LogWithUserScope(
                            log.UserId.Value,
                            LogLevel.Debug,
                            UserActions.FetchUser,
                            "Fetching user: {LogId}",
                            log.UserId.Value
                        );
                        user = await _userService.GetUser(log.UserId.Value);
                    }

                    return new LogViewModel
                    {
                        Id = log.Id,
                        LogLevel = log.LogLevel,
                        EventId = log.EventId.Id,
                        UserAction = log.UserAction,
                        Message = log.Message,
                        Timestamp = log.Timestamp,
                        User = user,
                    };
                })
                .ToList()
        );

        var model = new LogListViewModel { Logs = results.ToList() };

        // TODO: Add pagination and filtering to the logs list.
        return View(model);
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
}
