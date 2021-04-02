using System;
using System.Collections.Generic;
using System.Text;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services.PartyGoerSetting;

namespace SpotSync.Application.Services
{
    public class PartyGoerDetailsService : IPartyGoerDetailsService
    {
        private Dictionary<string, PartyGoerConfigurationSetting> _settings;

        public PartyGoerDetailsService()
        {
            _settings = new Dictionary<string, PartyGoerConfigurationSetting>();
        }

        public string GetMarket(PartyGoer partyGoer)
        {
            if (_settings.ContainsKey(partyGoer.Id))
            {
                return _settings[partyGoer.Id].Market;
            }

            return null;
        }

        public void SetMarket(PartyGoer partyGoer, string market)
        {
            if (_settings.ContainsKey(partyGoer.Id))
            {
                _settings[partyGoer.Id].Market = market;
            }
            else
            {
                _settings.Add(partyGoer.Id, new PartyGoerConfigurationSetting { Market = market });
            }

        }

        public void SetPerferredDeviceId(PartyGoer partyGoer, string perferredDeviceId)
        {
            if (_settings.ContainsKey(partyGoer.Id))
            {
                _settings[partyGoer.Id].PerferredDeviceId = perferredDeviceId;
            }
            else
            {
                _settings.Add(partyGoer.Id, new PartyGoerConfigurationSetting { PerferredDeviceId = perferredDeviceId });
            }
        }

        public string GetPerferredDeviceId(PartyGoer partyGoer)
        {
            if (_settings.ContainsKey(partyGoer.Id))
            {
                return _settings[partyGoer.Id].PerferredDeviceId;
            }

            return null;
        }
    }
}
