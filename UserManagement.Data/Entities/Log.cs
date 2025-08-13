using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserManagement.Models;

namespace UserManagement.Data.Entities;

public class Log
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public EventId EventId { get; set; }
    public UserActions? UserAction { get; set; }
    public LogLevel LogLevel { get; set; }
    public string Message { get; set; } = default!;
    public string? Exception { get; set; }
    public DateTime Timestamp { get; set; }
    public long? UserId { get; set; }
}
