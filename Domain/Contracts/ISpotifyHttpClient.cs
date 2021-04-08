using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface ISpotifyHttpClient
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
        Task<PartyGoerDetails> RequestAccessAndRefreshTokenFromSpotifyAsync(string code);
        Task<Errors.ServiceResult<UpdateSongError>> UpdateSongForPartyGoerAsync(PartyGoer user, List<string> songUris, int currentSongProgressInMs);
        Task<bool> UpdateSongForPartyGoerAsync(string partyGoerId, string songUri, int currentSongProgressInMs);
        Task<List<string>> GetUserTopTrackIdsAsync(string spotifyId, int count = 10);
        Task<List<string>> GetRecommendedTrackUrisAsync(string spotifyId, GetRecommendedSongs recommendedSongs);
        Task<List<Track>> GetRecommendedSongsAsync(string spotifyId, GetRecommendedSongs recommendedSongs);
        Task<List<Track>> GetUserTopTracksAsync(string spotifyId, int limit = 10);
        Task<string> GetUsersActiveDeviceAsync(string spotifyId);
        Task<IEnumerable<ISpotifyQueryResult>> QuerySpotifyAsync(PartyGoer user, string searchQuery, SpotifyQueryType queryType, int limit);
        Task<PartyGoerDetails> GetUserDetailsAsync(string spotifyId);
        Task TogglePlaybackAsync(PartyGoer partyGoer, PlaybackState state);
        Task<List<Device>> GetUserDevicesAsync(PartyGoer partyGoer);
        Task RefreshTokenForUserAsync(string partyGoerId);
    }
}
