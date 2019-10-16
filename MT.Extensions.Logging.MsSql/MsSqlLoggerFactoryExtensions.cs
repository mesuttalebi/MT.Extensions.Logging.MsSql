// Copyright (c) Mesut Talebi (mesut.talebi@gmail.com)

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace MT.Extensions.Logging.MsSql
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class MsSqlLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a debug logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext">Context to get Http Info</param>
        /// <param name="applicationName">The Application name that adds log, used when more than one application is used to log to same db</param>
        public static ILoggerFactory AddMsSql(this ILoggerFactory factory, string connectionString, IHttpContextAccessor httpContext, string applicationName = null)
        {
            return AddMsSql(factory, LogLevel.Error, connectionString, httpContext, applicationName);
        }

        /// <summary>
        /// Adds a debug logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext">Context to get Http Info</param>
        /// <param name="applicationName">The Application name that adds log, used when more than one application is used to log to same db</param>
        public static ILoggerFactory AddMsSql(this ILoggerFactory factory, Func<string, LogLevel, bool> filter
            , string connectionString, IHttpContextAccessor httpContext, string applicationName = null)
        {
            factory.AddProvider(new MsSqlLoggerProvider(filter, connectionString, httpContext, applicationName));
            return factory;
        }

        /// <summary>
        /// Adds a debug logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        /// <param name="connectionString">The connection string to MsSql Db to Log into</param>
        /// <param name="httpContext">Context to get Http Info</param>
        /// <param name="applicationName">The Application name that adds log, used when more than one application is used to log to same db</param>
        public static ILoggerFactory AddMsSql(this ILoggerFactory factory, LogLevel minLevel, string connectionString, 
            IHttpContextAccessor httpContext, string applicationName = null)
        {
            return AddMsSql(factory, (_, logLevel) => logLevel >= minLevel, connectionString, httpContext, applicationName);
        }
    }
}