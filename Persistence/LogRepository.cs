using Dapper;
using Npgsql;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence
{
    public class LogRepository : ILogRepository
    {
        private NpgsqlConnection _connection;
        public LogRepository(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        public async Task<string> LogExceptionAsync(Exception exception, string customMessage = null)
        {
            string sql = @"INSERT INTO public.exceptions(record_time, exception_message, stack_trace, custom_message, reference_id)
	                               VALUES (NOW(), @ExceptionMessage, @StackTrace, @CustomMessage, @ReferenceId);";

            string referenceId = Guid.NewGuid().ToString();


            await _connection.ExecuteAsync(sql, new
            {
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                CustomMessage = customMessage,
                ReferenceId = referenceId
            });
            return referenceId;
        }

        public async Task LogAppActivityAsync(string action)
        {
            await LogUserActivityAsync("SpotSync", action);
        }

        public async Task LogUserActivityAsync(string username, string action)
        {
            string sql = @"INSERT INTO public.activitylog(record_time, user_action, username)
                           VALUES (NOW(), @Action, @Username)";


            await _connection.ExecuteAsync(sql, new
            {
                Action = action,
                Username = username
            });

        }
    }
}

