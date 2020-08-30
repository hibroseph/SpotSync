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
        Task<bool> UpdatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee);
        bool IsUserHostingAParty(PartyGoer host);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        Task<bool> IsUserPartyingAsync(PartyGoer user);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        Task<bool> LeavePartyAsync(PartyGoer attendee);
    }
}
