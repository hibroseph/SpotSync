using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Domain.Contracts
{
    public interface IPartyService
    {
        Task<string> StartNewPartyAsync(PartyGoer host);
        Task<bool> EndPartyAsync(PartyGoer host);
        Task<bool> EndPartyAsync(string partyCode);
        Task UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer host);
        Task<bool> JoinPartyAsync(PartyCodeDTO partyCode, PartyGoer attendee);
        Task<bool> IsUserHostingAPartyAsync(PartyGoer host);
        Task<Party> GetPartyWithHostAsync(PartyGoer host);
        Task<bool> IsUserPartyingAsync(PartyGoer user);
        Task<Party> GetPartyWithAttendeeAsync(PartyGoer attendee);
        Task<Party> GetPartyWithCodeAsync(string partyCode);
        Task<bool> LeavePartyAsync(PartyGoer attendee);
        Task<List<Party>> GetTopPartiesAsync(int count);
        Task SyncListenerWithSongAsync(PartyGoer listener);
        Task<bool> AddNewSongToQueue(AddSongToQueueCommand request);
        Task<string> StartPartyAsync();
        Task UserWantsToSkipSong(PartyGoer partyGoer, string partyCode);
        Task TogglePlaybackStateAsync(string partyCode, PartyGoer listener);
        Task<List<Party>> GetAllPartiesAsync();
        Task<List<Track>> GenerateNewPlaylist(Party party, List<string> recommendedTrackUris, List<string> recommendedArtistUris);
        Task AddSomeTracksFromPlaylistToQueueAsync(PartyGoer partyGoer, string playlistId, int amount);

        #region Contributions
        Task UpdateContributionsAsync(string partyCode, List<Contribution> contributionsToAdd, List<Contribution> contributionsToRemove);
        Task<List<PartierContribution>> GetContributionsAsync(string partyCode, PartyGoer partier);
        Task RemoveContributionAsync(string partyCode, PartyGoer partier, Guid contributionId);
        #endregion
    }
}
