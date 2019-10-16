// Copyright (c) Mesut Talebi (mesut.talebi@gmail.com)

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MT.Extensions.Logging.MsSql
{
    /// <summary>
    /// A logger that writes messages in the debug output window only when a debugger is attached.
    /// </summary>
    public partial class MsSqlLogger : ILogger
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _name;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _context;
        private readonly string _application;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext">Context to get Http Info</param>
        /// <param name="application">the name of the application that logs, used if Many applications logs to same database</param>
        public MsSqlLogger(string name, string connectionString, IHttpContextAccessor httpContext, string application)
            : this(name, filter: null, connectionString: connectionString, httpContext: httpContext, application: application)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext">Context to get Http Info</param>
        public MsSqlLogger(string name, Func<string, LogLevel, bool> filter, string connectionString, IHttpContextAccessor httpContext, string application)
        {
            _name = string.IsNullOrEmpty(name) ? nameof(MsSqlLogger) : name;
            _filter = filter;
            _connectionString = connectionString;
            _context = httpContext;
            _application = application;
        }


        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return _filter == null || _filter(_name, logLevel);
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (logLevel < LogLevel.Error && string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";
            
            SqlSaveLog(logLevel,message, _name, exception, _context?.HttpContext, _application);
        }

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
