using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;

namespace UserManagement.Data.Logging;
internal class DatabaseLogger : ILogger
{
    private readonly DatabaseLoggerOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseLogger(DatabaseLoggerOptions options, IServiceProvider serviceProvider)
    {
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.MinLevel;   
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        string message = formatter(state, exception);
        string logLevelText = logLevel.ToString();

        var log = new Log
        {
            eventId = eventId,
            logLevel = logLevel,
            Message = message,
            Exception = exception?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        using (var scope = _serviceProvider.CreateScope())
        {
            var dataAccess = scope.ServiceProvider.GetRequiredService<IDataContext>();
            dataAccess.Create(log);
        }
    }



}
