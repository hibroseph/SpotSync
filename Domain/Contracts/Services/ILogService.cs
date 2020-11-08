using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface ILogService
    {
        Task<string> LogExceptionAsync(Exception exception, string customMessage);
        Task LogUserActivityAsync(string username, string action);
        Task LogUserActivityAsync(PartyGoer user, string action);
        Task LogAppActivityAsync(string activity);
        Task AddDescriptionToExceptionAsync(string message, string referenceId, string userId);
    }
}
