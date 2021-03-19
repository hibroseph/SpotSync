using System;
using System.Collections.Generic;
using System.Text;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services.PartyGoerSetting;

namespace SpotSync.Application.Services
{
    public class PartyGoerSettingsService : IPartyGoerSettingsService
    {
        private Dictionary<string, PartyGoerConfigurationSetting> _settings;

        public PartyGoerSettingsService()
        {
            _settings = new Dictionary<string, PartyGoerConfigurationSetting>();
        }

        public void SetConfigurationSetting(PartyGoer partyGoer, PartyGoerConfigurationSetting setting)
        {
            _settings[partyGoer.Id] = setting;
        }

        public PartyGoerConfigurationSetting GetConfigurationSetting(PartyGoer partyGoer)
        {
            if (_settings.ContainsKey(partyGoer.Id))
            {
                return _settings[partyGoer.Id];
            }

            return null;
        }
    }
}
