using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyService
    {
        string StartNewParty(PartyGoer host);
        Task<bool> EndPartyAsync(PartyGoer host);
        Task<bool> UpdateSongForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee);
        bool IsUserHostingAParty(PartyGoer host);
        Task<Party> GetPartyAsync(PartyGoer host);
    }
}
