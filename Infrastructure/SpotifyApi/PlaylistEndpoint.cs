using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System.Net.Http.Json;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpotSync.Domain.Contracts.Services;

namespace SpotSync.Infrastructure.SpotifyApi
{
    class PlaylistEndpoint
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private Dictionary<ApiEndpointType, SpotifyEndpoint> _apiEndpoints;
        private SpotifyToSpotibroModelMapper _mapper;
        private ILogService _logService;
        public PlaylistEndpoint(ISpotifyHttpClient spotifyHttpClient, ILogService logService, Dictionary<ApiEndpointType, SpotifyEndpoint> apiEndpoints)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
            _mapper = new SpotifyToSpotibroModelMapper();
            _apiEndpoints = apiEndpoints;
        }

        public async Task<SpotibroModels.PlaylistContents> GetPlaylistContentsAsync(PartyGoer user, string playlistId)
        {
            var parameters = new ApiParameters
            {
                Parameters = new Dictionary<string, string>
                {
                    {"fields", "items(track(id,name,album(images(url)),artists(name,id),explicit(),duration_ms()))" },
                    {"market",  user.GetMarket()}
                },
                Keys = new Dictionary<string, string>{
                    {"{playlist_id}", playlistId },
                }
            };

            var response = await _spotifyHttpClient.SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.GetPlaylistItems], parameters);

            response.EnsureSuccessStatusCode();

            var playlistItems = await response.Content.ReadFromJsonAsync<PlaylistItems>();

            playlistItems.Items.RemoveAll(p => p.Track == null);

            return new SpotibroModels.PlaylistContents()
            {
                Tracks = playlistItems.Items.Select(p => _mapper.Convert(p.Track)).ToList()
            };
        }

        public async Task<List<SpotibroModels.PlaylistSummary>> GetUsersPlaylistsAsync(PartyGoer user, int limit = 10, int offset = 0)
        {
            try
            {
                var parameters = new ApiParameters
                {
                    Parameters = new Dictionary<string, string>
                {
                    {"limit", limit.ToString() },
                    {"offset", offset.ToString() }
                }
                };

                var response = await _spotifyHttpClient.SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.GetUserPlaylists], parameters, null);

                response.EnsureSuccessStatusCode();

                PagedObject<UsersPlaylist> spotifyPlaylists = await response.Content.ReadFromJsonAsync<PagedObject<UsersPlaylist>>();
                List<SpotibroModels.PlaylistSummary> playlistSummaries = new List<SpotibroModels.PlaylistSummary>();

                foreach (var playlist in spotifyPlaylists.Items)
                {
                    playlistSummaries.Add(new SpotibroModels.PlaylistSummary { Name = playlist.Name, PlaylistCoverArtUrl = playlist.Images.FirstOrDefault().Url, Id = playlist.Id });
                }

                return playlistSummaries;
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetUsersPlaylistsAsync");
                return null;
            }
        }

        private string GetPlaylistImageUrl(JToken jsonImages)
        {
            return jsonImages.First?["url"].ToString();
        }

    }
}
