using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain;
using SpotSync.Domain.DTO;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyRepository
    {
        Party Get(Guid partyId);
        Task<Party> GetAsync(PartyCodeDTO partyCode);
        void Update(Party party);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        Task<bool> DeleteAsync(PartyGoer host);
        Task<bool> DeleteAsync(string partyCode);
        bool IsUserHostingAParty(PartyGoer host);
        Task<bool> IsUserInAPartyAsync(PartyGoer attendee);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        void CreateParty(Party item);
        bool LeaveParty(PartyGoer attendee);
        Task<List<Party>> GetPartiesWithMostListenersAsync(int count);
        Task<Party> GetPartyWithCode(string partyCode);
    }
}
