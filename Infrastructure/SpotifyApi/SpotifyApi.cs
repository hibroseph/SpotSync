using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using SpotSync.Domain.Contracts.SpotifyApi;
using System.Threading.Tasks;
using SpotSync.Domain.Contracts.SpotibroModels;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services;
using System.Net.Http;

namespace SpotSync.Infrastructure.SpotifyApi
{
    public class SpotifyApi : ISpotifyApi
    {
        private PlaylistEndpoint _playlistEndpoint;
        private PersonalizationEndpoint _personalizationEndpoint;

        public SpotifyApi(ISpotifyHttpClient spotifyHttpClient, ILogService logService)
        {
            var apiEndpoints = new Dictionary<ApiEndpointType, SpotifyEndpoint>
            {
                { ApiEndpointType.CurrentSong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/currently-playing", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.PlaySong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/play", HttpMethod = HttpMethod.Put } },
                { ApiEndpointType.Token, new SpotifyEndpoint { EndpointUrl = "https://accounts.spotify.com/api/token", HttpMethod = HttpMethod.Post } },
                { ApiEndpointType.UserInformation, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetTopTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/top/tracks", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetRecommendedTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/recommendations", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetUserDevices, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/devices", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.SearchSpotify, new SpotifyEndpoint{ EndpointUrl = "https://api.spotify.com/v1/search", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.PausePlayback, new SpotifyEndpoint {EndpointUrl = "https://api.spotify.com/v1/me/player/pause", HttpMethod = HttpMethod.Put } },
                { ApiEndpointType.GetUserPlaylists, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/playlists", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetPlaylistItems, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/playlists/{playlist_id}/tracks", HttpMethod = HttpMethod.Get,
                Keys = new List<string> {"{playlist_id}"} } },
                { ApiEndpointType.ArtistInformation, new SpotifyEndpoint { EndpointUrl =  "https://api.spotify.com/v1/artists/{id}", HttpMethod = HttpMethod.Get,
                Keys = new List<string> { "{id}"} } }, {ApiEndpointType.ArtistTopTracks, new SpotifyEndpoint{ EndpointUrl = "https://api.spotify.com/v1/artists/{id}/top-tracks", HttpMethod = HttpMethod.Get,
                Keys = new List<string> {"{id}" }  } },
                { ApiEndpointType.UsersTopArtists, new SpotifyEndpoint{ EndpointUrl ="https://api.spotify.com/v1/me/top/artists", HttpMethod = HttpMethod.Get} }
            };

            _playlistEndpoint = new PlaylistEndpoint(spotifyHttpClient, logService, apiEndpoints);
            _personalizationEndpoint = new PersonalizationEndpoint(spotifyHttpClient, logService, apiEndpoints);
        }

        public async Task<PlaylistContents> GetPlaylistContentsAsync(PartyGoer partyGoer, string playlistId)
        {
            return await _playlistEndpoint.GetPlaylistContentsAsync(partyGoer, playlistId);
        }

        public async Task<List<PlaylistSummary>> GetUsersPlaylistsAsync(PartyGoer partyGoer, int limit, int offset)
        {
            return await _playlistEndpoint.GetUsersPlaylistsAsync(partyGoer, limit, offset);
        }

        public async Task<List<Artist>> GetUsersTopArtistsAsync(PartyGoer partyGoer, int amount = 10)
        {
            if (amount < 0 || amount > 50)
            {
                throw new Exception($"Amount must be between 0 and 50. Currently it is {amount}");
            }

            return await _personalizationEndpoint.GetUsersTopArtistsAsync(partyGoer, amount);
        }
    }
}
