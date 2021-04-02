using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.Services.PartyGoerSetting
{
    public interface IPartyGoerDetailsService
    {
        void SetMarket(PartyGoer partyGoer, string market);
        string GetMarket(PartyGoer partyGoer);

        void SetPerferredDeviceId(PartyGoer partyGoer, string perferredDeviceId);
        string GetPerferredDeviceId(PartyGoer partyGoer);
    }
}
