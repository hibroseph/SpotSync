using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain;
using SpotSync.Domain.DTO;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyRepository
    {
        void UpdateParty(Party party);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        bool DeletePartyWithHost(PartyGoer host);
        Task<bool> DeleteAsync(string partyCode);
        bool IsUserHostingAParty(PartyGoer host);
        Task<bool> IsUserInAPartyAsync(PartyGoer attendee);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        void CreateParty(Party item);
        bool LeaveParty(PartyGoer attendee);
        Task<List<Party>> GetPartiesWithMostListenersAsync(int count);
        Task<Party> GetPartyWithCodeAsync(string partyCode);
        Task RemoveHostFromPartyAsync(PartyGoer host);
        Task<List<Party>> GetAllPartiesAsync();
    }
}
