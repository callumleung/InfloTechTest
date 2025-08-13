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
    private readonly IExternalScopeProvider _scopeProvider;

    public DatabaseLogger(
        DatabaseLoggerOptions options,
        IServiceProvider serviceProvider,
        IExternalScopeProvider scopeProvider
    )
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _scopeProvider = scopeProvider;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.MinLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
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

        // Try to extract UserId and action from the scope
        long? userId = null;
        UserActions? userAction = null;
        _scopeProvider.ForEachScope(
            (scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> values)
                {
                    var userIdEntry = values.FirstOrDefault(kv => kv.Key == "UserId");
                    if (userIdEntry.Value is long id)
                    {
                        userId = id;
                    }

                    var userActionEntry = values.FirstOrDefault(kv => kv.Key == "UserAction");
                    if (userActionEntry.Value is UserActions value)
                    {
                        userAction = value;
                    }
                }
            },
            state
        );

        var log = new Log
        {
            EventId = eventId,
            LogLevel = logLevel,
            UserAction = userAction,
            Message = message,
            Exception = exception?.ToString(),
            Timestamp = DateTime.UtcNow,
            UserId = userId,
        };

        using (var scope = _serviceProvider.CreateScope())
        {
            var dataAccess = scope.ServiceProvider.GetRequiredService<IDataContext>();
            dataAccess.Create(log);
        }
    }
}
