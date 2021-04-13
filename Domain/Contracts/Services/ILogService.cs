using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface ILogService
    {
        Task<string> LogExceptionAsync(Exception exception, string customMessage);
        Task LogUserActivityAsync(PartyGoer user, string action);
        Task LogAppActivityAsync(string activity);
    }
}
