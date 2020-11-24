// Copyright (c) Mesut Talebi (mesut.talebi@gmail.com)

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MT.Extensions.Logging.MsSql.Model;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MT.Extensions.Logging.MsSql
{
    public partial class MsSqlLogger
    {
        private void SqlSaveLog(LogLevel logLevel, string message, string category, Exception ex, HttpContext context, string application)
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd;

                if (ex == null)
                {
                    cmd = GetSqlCommandFromMessage(logLevel, message, category, context, application);                    
                }
                else
                {
                    cmd = GetSqlCommandFromError(new Log(ex, context) {Category = category, Application = application}, message, out string logId);

                    if (context != null)
                    {
                        if (context.Items.ContainsKey("ExceptionLogId"))
                            context.Items["ExceptionLogId"] = logId;
                        else
                            context.Items.Add("ExceptionLogId", logId);
                    }

                }

                using (cmd)
                {
                    cmd.Connection = conn;
                    conn.Open();
                    var task = cmd.ExecuteNonQueryAsync();
                    task.Wait();
                }

                
            }            
        }

        private SqlCommand GetSqlCommandFromMessage(LogLevel logLevel, string message, string category, HttpContext context, string application)
        {
            var id = Guid.NewGuid();

            var command = new SqlCommand("spInsertLog")
            {
                CommandType = CommandType.StoredProcedure
            };

            var parameters = command.Parameters;

            parameters.Add("@LogId", SqlDbType.UniqueIdentifier).Value = id;
            parameters.Add("@Category", SqlDbType.NVarChar).Value = category;
            parameters.Add("@Type", SqlDbType.NVarChar, 100).Value = logLevel.ToString();
            parameters.Add("@Source", SqlDbType.NVarChar, 60).Value = string.Empty;            
            parameters.Add("@FileName", SqlDbType.NVarChar, 400).Value = string.Empty;            
            parameters.Add("@Message", SqlDbType.NVarChar, 500).Value = message;
            parameters.Add("@User", SqlDbType.NVarChar, 50).Value = context?.User.Identity.Name ?? string.Empty;
            parameters.Add("@ExceptionDetail", SqlDbType.NVarChar, -1).Value = string.Empty;
            parameters.Add("@StatusCode", SqlDbType.Int).Value = context?.Response.StatusCode ?? 0;
            parameters.Add("@TimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;
            parameters.Add("@StackTrace", SqlDbType.NVarChar, 4000).Value = string.Empty;
            parameters.Add("@Application", SqlDbType.NVarChar, 100).Value = application;            

            return command;
        }

        private static SqlCommand GetSqlCommandFromError(Log error, string message, out string logId)
        {
            var errorJson = error.Serialize();
            var id = Guid.NewGuid();
            logId = id.ToString();

            var command = new SqlCommand("spInsertLog")
            {
                CommandType = CommandType.StoredProcedure
            };

            var parameters = command.Parameters;

            parameters.Add("@LogId", SqlDbType.UniqueIdentifier).Value = id;
            parameters.Add("@Category", SqlDbType.NVarChar).Value = error.Category;
            parameters.Add("@Type", SqlDbType.NVarChar, 100).Value = error.Type;
            parameters.Add("@Source", SqlDbType.NVarChar, 60).Value = error.Source;
            parameters.Add("@FileName", SqlDbType.NVarChar, 400).Value = error.FileName;
            parameters.Add("@Message", SqlDbType.NVarChar, 500).Value = (string.IsNullOrEmpty(message) ? "" : message + " - ") +error.Message;
            parameters.Add("@User", SqlDbType.NVarChar, 50).Value = error.User;
            parameters.Add("@ExceptionDetail", SqlDbType.NVarChar, -1).Value = errorJson;
            parameters.Add("@StatusCode", SqlDbType.Int).Value = error.StatusCode;
            parameters.Add("@TimeUtc", SqlDbType.DateTime).Value = error.Time;
            parameters.Add("@StackTrace", SqlDbType.NVarChar, 4000).Value = error.StackTrace;
            parameters.Add("@Application", SqlDbType.NVarChar, 100).Value = error.Application;
            parameters.Add("@RequestId", SqlDbType.NVarChar).Value = error.RequestId;

            return command;
        }
    }
}
