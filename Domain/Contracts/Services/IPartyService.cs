using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyService
    {
        Task<string> StartNewPartyAsync(PartyGoer host);
        Task<bool> EndPartyAsync(PartyGoer host);
        Task<bool> EndPartyAsync(string partyCode);
        Task<bool> UpdatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<List<Track>> CreatePartyPlaylistForEveryoneInPartyAsync(Party party, PartyGoer user);
        Task<ServiceResult<UpdateSongError>> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee);
        Task<bool> IsUserHostingAPartyAsync(PartyGoer host);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        Task<bool> IsUserPartyingAsync(PartyGoer user);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        Task<Party> GetPartyWithCodeAsync(string partyCode);
        Task<bool> LeavePartyAsync(PartyGoer attendee);
        Task<string> StartPartyWithSeedSongsAsync(List<string> seedTrackUris, PartyGoer host);
        Task<List<Party>> GetTopParties(int count);
        Task SyncUserWithSongAsync(PartyGoer listener);
        Task<bool> RearrangeQueue(RearrangeQueueRequest request);
        Task<bool> AddNewSongToQueue(AddSongToQueueRequest request);
        Task<string> StartPartyAsync();
        Task UserWantsToSkipSong(PartyGoer partyGoer, string partyCode);
        Task TogglePlaybackStateAsync(string partyCode, PartyGoer listener);
    }
}
