using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Repositories
{
    public interface ILogRepository
    {
        Task<string> LogExceptionAsync(Exception exception, string CustomMessage);
        Task LogUserActivityAsync(string username, string action);
        Task LogAppActivityAsync(string action);

    }
}
