using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserManagement.Data.Logging;
internal class DatabaseLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    public readonly DatabaseLoggerOptions Options;
    private readonly IServiceProvider _serviceProvider;
    private IExternalScopeProvider? _scopeProvider;

    public DatabaseLoggerProvider(IOptions<DatabaseLoggerOptions> options, IServiceProvider serviceProvider)
    {
        Options = options.Value;
        _serviceProvider = serviceProvider;
    }

    public ILogger CreateLogger(string categoryName) => new DatabaseLogger(Options, _serviceProvider, _scopeProvider ?? new LoggerExternalScopeProvider());

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    => _scopeProvider = scopeProvider;

    public void Dispose() { }
}
