using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.Services.PartyGoerSetting
{
    public interface IPartyGoerSettingsService
    {
        PartyGoerConfigurationSetting GetConfigurationSetting(PartyGoer partyGoer);
        void SetConfigurationSetting(PartyGoer partyGoer, PartyGoerConfigurationSetting setting);
    }
}
