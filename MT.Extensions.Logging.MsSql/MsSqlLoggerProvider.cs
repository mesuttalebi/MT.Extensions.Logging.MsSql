// Copyright (c) Mesut Talebi (mesut.talebi@gmail.com)

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace MT.Extensions.Logging.MsSql
{
    /// <summary>
    /// The provider for the <see cref="MT.Extensions.Logging.MsSql.MsSqlLogger"/>.
    /// </summary>
    public class MsSqlLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _context;
        private readonly string _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext"></param>
        /// <param name="application">the Application name that adds log, used when more than one application uses to logs to same db</param>
        public MsSqlLoggerProvider(Func<string, LogLevel, bool> filter, string connectionString, IHttpContextAccessor httpContext, string application)
        {
            _filter = filter;
            _connectionString = connectionString;
            _context = httpContext;
            _application = application;
        }

        /// <inheritdoc /> 
        public ILogger CreateLogger(string name)
        {
            return new MT.Extensions.Logging.MsSql.MsSqlLogger(name, _filter, _connectionString, _context, _application);
        }

        public void Dispose()
        {            
        }
    }
}
