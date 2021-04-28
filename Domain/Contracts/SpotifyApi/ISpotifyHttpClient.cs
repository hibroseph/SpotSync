using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain.Contracts.SpotifyApi
{
    public interface ISpotifyHttpClient
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
        Task<User> RequestAccessAndRefreshTokenFromSpotifyAsync(string code);
        Task<Errors.ServiceResult<UpdateSongError>> UpdateSongForPartyGoerAsync(PartyGoer user, List<string> songUris, int currentSongProgressInMs);
        Task<List<Track>> GetRecommendedTracksAsync(PartyGoer partyGoer, RecommendedTracksSeed recommendedSongs);
        Task<List<SpotibroModels.Track>> GetUserTopTracksAsync(string spotifyId, int limit = 10);
        Task<string> GetUsersActiveDeviceAsync(string spotifyId);
        Task<IEnumerable<ISpotifyQueryResult>> QuerySpotifyAsync(PartyGoer user, string searchQuery, SpotifyQueryType queryType, int limit);
        Task<User> GetUserDetailsAsync(string spotifyId);
        Task TogglePlaybackAsync(PartyGoer partyGoer, PlaybackState state);
        Task<List<Device>> GetUserDevicesAsync(PartyGoer partyGoer);
        Task RefreshTokenForUserAsync(string partyGoerId);
        Task<SpotibroModels.ArtistInformation> GetArtistInformationAsync(PartyGoer partyGoer, string artistId);
        Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint endpoint, ApiParameters parameters);
        Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false);
        Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, ApiParameters queryStringParameters, object requestBodyParameters);
        Task<HttpResponseMessage> SendHttpRequestAsync(string spotifyId, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false);


    }
}
