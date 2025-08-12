using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Logging;

namespace UserManagement.Data.Extensions;
public static class DatabaseLoggerExtensions
{
    public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, Action<DatabaseLoggerOptions> configure)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
        builder.Services.Configure(configure);


        return builder;
    }
}
