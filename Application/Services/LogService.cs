using SpotSync.Domain;
using SpotSync.Domain.Contracts.Repositories;
using SpotSync.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class LogService : ILogService
    {
        private ILogRepository _repository;
        public LogService(ILogRepository repository)
        {
            _repository = repository;
        }
        public async Task<string> LogExceptionAsync(Exception exception, string customMsg)
        {
            return await _repository.LogExceptionAsync(exception, customMsg);
        }

        public async Task LogUserActivityAsync(string username, string activity)
        {
            await _repository.LogUserActivityAsync(username, activity);
        }

        public async Task LogUserActivityAsync(PartyGoer user, string activity)
        {
            await _repository.LogUserActivityAsync(user.Id, activity);
        }

        public async Task LogAppActivityAsync(string activity)
        {
            await _repository.LogAppActivityAsync(activity);
        }

        public async Task AddDescriptionToExceptionAsync(string message, string referenceId, string userId)
        {
            if (message.Length > 2000)
            {
                message = message.Substring(0, 1999);
            }

            await _repository.AddDescriptionToExceptionAsync(message, referenceId, userId);
        }
    }
}
