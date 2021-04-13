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

        public async Task LogUserActivityAsync(PartyGoer user, string activity)
        {
            await _repository.LogUserActivityAsync(user.GetId(), activity);
        }

        public async Task LogAppActivityAsync(string activity)
        {
            await _repository.LogAppActivityAsync(activity);
        }
    }
}
