using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserManagement.Data.Logging;
public class DatabaseLoggerOptions
{
    public LogLevel MinLevel { get; set; } = LogLevel.Trace;

}
