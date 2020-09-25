using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyService
    {
        Task<string> StartNewPartyAsync(PartyGoer host);
        Task<bool> EndPartyAsync(PartyGoer host);
        Task<bool> UpdatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<List<Song>> CreatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer user);
        Task<ServiceResult<UpdateSongError>> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee);
        Task<bool> IsUserHostingAPartyAsync(PartyGoer host);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        Task<bool> IsUserPartyingAsync(PartyGoer user);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        Task<bool> LeavePartyAsync(PartyGoer attendee);
    }
}
