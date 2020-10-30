using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SpotSync.Infrastructure
{
    public class SpotifyHttpClient : ISpotifyHttpClient
    {
        private IHttpClient _httpClient;
        private ISpotifyAuthentication _spotifyAuthentication;
        private ILogService _logService;
        private Dictionary<ApiEndpointType, SpotifyEndpoint> _apiEndpoints;

        public SpotifyHttpClient(ISpotifyAuthentication spotifyAuthentication, IHttpClient httpClient, ILogService logService)
        {
            _httpClient = httpClient;
            _spotifyAuthentication = spotifyAuthentication;
            _logService = logService;
            _apiEndpoints = new Dictionary<ApiEndpointType, SpotifyEndpoint>
            {
                { ApiEndpointType.CurrentSong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/currently-playing", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.PlaySong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/play", HttpMethod = HttpMethod.Put } },
                { ApiEndpointType.Token, new SpotifyEndpoint { EndpointUrl = "https://accounts.spotify.com/api/token", HttpMethod = HttpMethod.Post } },
                { ApiEndpointType.UserInformation, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetTopTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/top/tracks", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetRecommendedTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/recommendations", HttpMethod = HttpMethod.Get} },
                { ApiEndpointType.GetUserDevices, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/devices", HttpMethod = HttpMethod.Get} },
                { ApiEndpointType.SearchSpotify, new SpotifyEndpoint{ EndpointUrl = "https://api.spotify.com/v1/search", HttpMethod = HttpMethod.Get} }
            };
        }

        public async Task<List<Song>> GetUserTopTracksAsync(string spotifyId, int limit = 10)
        {
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetTopTracks], $"limit={limit}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Song> recommendedTrackUris = new List<Song>();
            foreach (var item in json["items"])
            {
                recommendedTrackUris.Add(new Song
                {
                    Title = item["name"].ToString(),
                    Artist = item["artists"].First()["name"].ToString(),
                    TrackUri = item["id"].ToString(),
                    Length = item["duration_ms"].Value<int>(),
                    AlbumImageUrl = item["album"]["images"].First["url"].ToString()
                });
            }

            return recommendedTrackUris;
        }

        public async Task<List<string>> GetRecommendedTrackUrisAsync(string spotifyId, List<string> seedTrackIds, float minimumEnergy)
        {
            if (seedTrackIds.Count > 5)
                throw new ArgumentException("Seed tracks cannot exeed 5");

            if (minimumEnergy > 1 && minimumEnergy < 0)
                throw new ArgumentException("Minimum Energy must be between 0 and 1");

            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetRecommendedTracks], $"seed_tracks={HttpUtility.UrlEncode(ConvertToCommaDelimitedString(seedTrackIds))}&min_energy={minimumEnergy}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<string> recommendedTrackUris = new List<string>();
            foreach (var item in json["tracks"])
            {
                recommendedTrackUris.Add(item["uri"].ToString());
            }

            return recommendedTrackUris;
        }

        public async Task<List<Song>> GetRecommendedSongsAsync(string spotifyId, List<string> seedTrackIds, float minimumEnergy)
        {
            if (seedTrackIds.Count > 5)
                throw new ArgumentException("Seed tracks cannot exeed 5");

            if (minimumEnergy > 1 && minimumEnergy < 0)
                throw new ArgumentException("Minimum Energy must be between 0 and 1");

            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetRecommendedTracks], $"seed_tracks={HttpUtility.UrlEncode(ConvertToCommaDelimitedString(seedTrackIds))}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Song> recommendedSongs = new List<Song>();
            foreach (var item in json["tracks"])
            {
                recommendedSongs.Add(new Song
                {
                    TrackUri = item["uri"].ToString(),
                    Title = item["name"].ToString(),
                    Artist = item["artists"].First["name"].ToString(),
                    Length = item["duration_ms"].Value<int>(),
                    AlbumImageUrl = item["album"]["images"].First["url"].ToString()
                });
            }

            return recommendedSongs.ToList();
        }

        private string ConvertToCommaDelimitedString(List<string> items)
        {
            return string.Join(",", items);
        }

        public async Task<string> RequestAccessAndRefreshTokenFromSpotifyAsync(string code)
        {
            List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _spotifyAuthentication.RedirectUrl),
                new KeyValuePair<string, string>("client_id", _spotifyAuthentication.ClientId),
                new KeyValuePair<string, string>("client_secret", _spotifyAuthentication.ClientSecret)
            };

            HttpResponseMessage response = null;

            using (var requestMessage = new HttpRequestMessage(_apiEndpoints[ApiEndpointType.Token].HttpMethod, _apiEndpoints[ApiEndpointType.Token].EndpointUrl))
            {
                requestMessage.Content = new FormUrlEncodedContent(properties);

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response is null)
            {
                throw new Exception("The response from requesting the access and refresh token was null");
            }
            // TODO: Add logic to know if it spotifys problem or ours
            if (response.IsSuccessStatusCode)
            {
                JObject accessTokenBody = JObject.Parse(await response.Content.ReadAsStringAsync());

                string accessToken = accessTokenBody["access_token"].ToString();

                string currentUserId = await GetCurrentUserIdAsync(accessToken);

                await _spotifyAuthentication.AddAuthenticatedPartyGoerAsync(currentUserId, accessToken,
                accessTokenBody["refresh_token"].ToString(),
                Convert.ToInt32(accessTokenBody["expires_in"])
                );

                return currentUserId;
            }

            return null;
        }

        private async Task<string> GetCurrentUserIdAsync(string accessToken)
        {

            HttpResponseMessage response = null;

            using (var requestMessage = new HttpRequestMessage(_apiEndpoints[ApiEndpointType.UserInformation].HttpMethod, _apiEndpoints[ApiEndpointType.UserInformation].EndpointUrl))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception("Could not get user details while logging a user in.");
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            JObject currentUser = JObject.Parse(responseContent);

            return currentUser["id"].ToString();
        }

        public async Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(partyGoerId, _apiEndpoints[ApiEndpointType.CurrentSong]);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            JObject currentSong = JObject.Parse(responseContent);

            if (currentSong["currently_playing_type"].ToString().Equals("track", StringComparison.OrdinalIgnoreCase))
            {
                return new CurrentSongDTO
                {
                    Artist = currentSong["item"]["artists"][0]["name"].ToString(),
                    Album = currentSong["item"]["album"]["name"].ToString(),
                    ProgressMs = Convert.ToInt32(currentSong["progress_ms"].ToString()),
                    Title = currentSong["item"]["name"].ToString(),
                    TrackUri = currentSong["item"]["uri"].ToString(),
                    AlbumArtUrl = currentSong["item"]["album"]["images"][0]["url"].ToString()
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<List<string>> GetUserTopTrackIdsAsync(string spotifyId, int count = 10)
        {
            await RefreshTokenForUserAsync(spotifyId);

            HttpResponseMessage response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetTopTracks], $"limit={count}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<string> trackUris = new List<string>();

            foreach (var item in json["items"])
            {
                trackUris.Add(item["id"].ToString());
            }

            return trackUris;
        }

        public async Task<List<Song>> SearchSpotifyAsync(string spotifyId, string query)
        {
            await RefreshTokenForUserAsync(spotifyId);

            HttpResponseMessage response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(query)}&type={HttpUtility.UrlEncode("track,artist")}&limit=10", true);

            response.EnsureSuccessStatusCode();

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Song> songs = new List<Song>();

            foreach (var item in json["tracks"]["items"])
            {
                songs.Add(new Song
                {
                    Artist = item["artists"].First()["name"].ToString(),
                    AlbumImageUrl = item["album"]["images"].First()["url"].ToString(),
                    Length = item["duration_ms"].Value<int>(),
                    Title = item["name"].ToString(),
                    TrackUri = item["uri"].ToString()
                });
            }

            return songs;
        }

        public async Task<ServiceResult<UpdateSongError>> UpdateSongForPartyGoerAsync(string partyGoerId, List<string> songUris, int currentSongProgressInMs)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(partyGoerId, _apiEndpoints[ApiEndpointType.PlaySong], new StartUserPlaybackSong
            {
                uris = songUris,
                position_ms = currentSongProgressInMs
            });

            ServiceResult<UpdateSongError> error = new ServiceResult<UpdateSongError>();

            if (response.IsSuccessStatusCode)
            {
                return error;
            }
            else
            {
                await _logService.LogExceptionAsync(new Exception($"Unable to update song for {partyGoerId}"), await response.Content.ReadAsStringAsync());
                // TODO: Check status codes and add specific messaging for status codes based on Spotifys API
                error.AddError(new UpdateSongError($"Unable to update song for {partyGoerId}"));
                return error;
            }
        }

        public async Task<bool> UpdateSongForPartyGoerAsync(string partyGoerId, string songUri, int currentSongProgressInMs)
        {
            if (!songUri.StartsWith("spotify:track:", StringComparison.OrdinalIgnoreCase))
            {
                songUri = $"spotify:track:{songUri}";
            }

            HttpResponseMessage response = await SendHttpRequestAsync(partyGoerId, _apiEndpoints[ApiEndpointType.PlaySong], new StartUserPlaybackSong
            {
                uris = new List<string> { songUri },
                position_ms = currentSongProgressInMs
            });

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // device might be turned off lets
            if ((await response.Content.ReadAsStringAsync()).Contains("NO_ACTIVE_DEVICE"))
            {
                throw new NoActiveDeviceException();
            }
            else
            {
                throw new Exception($"Unable to update song for {partyGoerId}. Message from Spotify: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<string> GetUsersActiveDeviceAsync(string spotifyId)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetUserDevices]);

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

                foreach (var item in json["devices"])
                {
                    if (item["is_active"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        return item["name"].ToString();
                    }
                }
                return null;

            }
            else
            {
                await _logService.LogExceptionAsync(new Exception("Unable to get users devices from Spotify"), await response.Content.ReadAsStringAsync());
                return null;
            }
        }

        private async Task RefreshTokenForUserAsync(string partyGoerId)
        {
            if (await _spotifyAuthentication.DoesAccessTokenNeedRefreshAsync(partyGoerId))
            {
                await RequestNewAccessToken(_spotifyAuthentication.GetRefreshTokenForPartyGoer(partyGoerId));
            }
        }

        private async Task RequestNewAccessToken(string refreshToken)
        {
            List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", _spotifyAuthentication.ClientId),
                new KeyValuePair<string, string>("client_secret", _spotifyAuthentication.ClientSecret)
            };

            HttpResponseMessage response = null;

            using (var requestMessage = new HttpRequestMessage(_apiEndpoints[ApiEndpointType.Token].HttpMethod, _apiEndpoints[ApiEndpointType.Token].EndpointUrl))
            {
                requestMessage.Content = new FormUrlEncodedContent(properties);

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response is null)
            {
                throw new Exception("The response from request a new access token with a refresh token was null");
            }

            if (response.IsSuccessStatusCode)
            {
                JObject accessTokenBody = JObject.Parse(await response.Content.ReadAsStringAsync());

                string accessToken = accessTokenBody["access_token"].ToString();

                string currentUserId = await GetCurrentUserIdAsync(accessToken);

                await _spotifyAuthentication.RefreshAccessTokenForPartyGoerAsync(currentUserId, accessToken,
                Convert.ToInt32(accessTokenBody["expires_in"])
                );
            }

        }

        private async Task<HttpResponseMessage> SendHttpRequestAsync(string spotifyId, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false)
        {
            await RefreshTokenForUserAsync(spotifyId);

            using (var requestMessage = new HttpRequestMessage(spotifyEndpoint.HttpMethod, spotifyEndpoint.EndpointUrl + (useQueryString ? $"?{content}" : string.Empty)))
            {
                requestMessage.Headers.Authorization = await _spotifyAuthentication.GetAuthenticationHeaderForPartyGoerAsync(spotifyId);

                if (content != null)
                    if (!useQueryString)
                        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(content));


                return await _httpClient.SendAsync(requestMessage);
            }
        }
    }
}
