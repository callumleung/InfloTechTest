using System;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Web.Models.Logs;

public class LogViewModel
{
    public long Id { get; set; }
    public EventId EventId { get; set; }
    public UserActions? UserAction { get; set; }
    public LogLevel? LogLevel { get; set; }
    public string Message { get; set; } = default!;
    public string? Exception { get; set; }
    public DateTime Timestamp { get; set; }
    public User? User { get; set; }

}
