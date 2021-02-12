using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Contracts.Services.PartyGoerSetting;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
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
        private IPartyGoerSettingsService _partyGoerSettingsService;
        private Dictionary<ApiEndpointType, SpotifyEndpoint> _apiEndpoints;

        public SpotifyHttpClient(ISpotifyAuthentication spotifyAuthentication, IHttpClient httpClient, ILogService logService, IPartyGoerSettingsService partyGoerSettingsService)
        {
            _httpClient = httpClient;
            _spotifyAuthentication = spotifyAuthentication;
            _logService = logService;
            _partyGoerSettingsService = partyGoerSettingsService;
            _apiEndpoints = new Dictionary<ApiEndpointType, SpotifyEndpoint>
            {
                { ApiEndpointType.CurrentSong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/currently-playing", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.PlaySong, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/play", HttpMethod = HttpMethod.Put } },
                { ApiEndpointType.Token, new SpotifyEndpoint { EndpointUrl = "https://accounts.spotify.com/api/token", HttpMethod = HttpMethod.Post } },
                { ApiEndpointType.UserInformation, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetTopTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/top/tracks", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetRecommendedTracks, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/recommendations", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetUserDevices, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/player/devices", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.SearchSpotify, new SpotifyEndpoint{ EndpointUrl = "https://api.spotify.com/v1/search", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.PausePlayback, new SpotifyEndpoint {EndpointUrl = "https://api.spotify.com/v1/me/player/pause", HttpMethod = HttpMethod.Put } }
            };
        }

        public async Task TogglePlaybackAsync(PartyGoer partyGoer, PlaybackState state)
        {
            var response = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.PausePlayback]);

            EnsureSuccessfulResponse(response);
        }

        public async Task<PartyGoerDetails> GetUserDetailsAsync(string spotifyId)
        {
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.UserInformation]);

            EnsureSuccessfulResponse(response);

            JObject jObject = await GetJObjectContentFromResponseAsync(response);

            return new PartyGoerDetails
            {
                ShouldFilterExplicitSongs = jObject["explicit_content"]["filter_enabled"].Value<bool>(),
                Id = jObject["id"].ToString()
            };
        }

        public async Task<IEnumerable<ISpotifyQueryResult>> QuerySpotifyAsync(PartyGoer user, string searchQuery, SpotifyQueryType queryType, int limit)
        {
            try
            {
                switch (queryType)
                {
                    case SpotifyQueryType.Track:
                        return await QuerySpotifyForTrackAsync(user, searchQuery, limit);
                    case SpotifyQueryType.Artist:
                        return await QuerySpotifyForArtistAsync(user, searchQuery, limit);
                    case SpotifyQueryType.Album:
                        return await QuerySpotifyForAlbumAsync(user, searchQuery, limit);
                    case SpotifyQueryType.Playlist:
                    //return await QuerySpotifyForPlaylistAsync(user, searchQuery, limit);
                    case SpotifyQueryType.All:
                    //return await QuerySpotifyAsync<T>(user, searchQuery);
                    default:
                        throw new ArgumentException($"Argument SpotifyQuertyType of {queryType} not handled");
                }
            }
            catch (Exception ex)
            {
                // TODO: Add custom exception to catch in clients to know that there was a problem with Spotifys API
                throw new Exception($"Error occurred while trying to query Spotify with query {searchQuery}", ex);
            }
        }

        private async Task<IEnumerable<SpotifyTrackQueryResult>> QuerySpotifyForTrackAsync(PartyGoer user, string searchQuery, int limit)
        {
            var response = await SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(searchQuery)}&type=track&limit={limit}", true);

            EnsureSuccessfulResponse(response);

            List<Track> tracks = await ReadFullTrackJsonObjectFromResponseAsync(response);

            return tracks.Select(p => new SpotifyTrackQueryResult { Uri = p.Uri, Artist = p.Artist, Length = p.Length, Name = p.Name, Explicit = p.Explicit });
        }

        private async Task<List<Track>> ReadFullTrackJsonObjectFromResponseAsync(HttpResponseMessage response)
        {
            JObject json = await GetJObjectContentFromResponseAsync(response);

            List<Track> tracks = new List<Track>();

            foreach (var item in json["tracks"]["items"])
            {
                tracks.Add(new Track
                {
                    Name = item["name"].ToString(),
                    Artist = item["artists"].First()["name"].ToString(),
                    Uri = item["uri"].ToString(),
                    Length = item["duration_ms"].Value<int>(),
                    AlbumImageUrl = item["album"]["images"].First["url"].ToString(),
                    Explicit = item["explicit"].Value<bool>()
                });
            }

            return tracks;
        }

        private async Task<IEnumerable<SpotifyArtistQueryResult>> QuerySpotifyForArtistAsync(PartyGoer user, string searchQuery, int limit)
        {
            var response = await SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(searchQuery)}&type=artist&limit={limit}", true);

            EnsureSuccessfulResponse(response);

            return await ReadSimplifiedArtistJsonObjectFromResponseAsync(response);
        }

        private async void EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error code from Spotify while using their api {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        private async Task<IEnumerable<SpotifyArtistQueryResult>> ReadSimplifiedArtistJsonObjectFromResponseAsync(HttpResponseMessage response)
        {
            JObject json = await GetJObjectContentFromResponseAsync(response);

            List<SpotifyArtistQueryResult> artists = new List<SpotifyArtistQueryResult>();

            foreach (var item in json["artists"]["items"])
            {
                artists.Add(new SpotifyArtistQueryResult
                {
                    Name = item["name"].ToString(),
                    Uri = item["uri"].ToString()
                });
            }

            return artists;
        }

        private async Task<JObject> GetJObjectContentFromResponseAsync(HttpResponseMessage response)
        {
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<IEnumerable<SpotifyAlbumQueryResult>> QuerySpotifyForAlbumAsync(PartyGoer user, string searchQuery, int limit)
        {
            var response = await SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(searchQuery)}&type=album&limit={limit}", true);

            response.EnsureSuccessStatusCode();

            return await ReadSimplifiedAlbumJsonObjectFromResponseAsync(response);
        }

        private async Task<IEnumerable<SpotifyAlbumQueryResult>> ReadSimplifiedAlbumJsonObjectFromResponseAsync(HttpResponseMessage response)
        {
            JObject json = await GetJObjectContentFromResponseAsync(response);

            List<SpotifyAlbumQueryResult> results = new List<SpotifyAlbumQueryResult>();

            foreach (var item in json["albums"]["items"])
            {
                results.Add(new SpotifyAlbumQueryResult
                {
                    Artist = item["artists"].First()["name"].ToString(),
                    Name = item["name"].ToString(),
                    Uri = item["uri"].ToString()
                });
            }

            return results;
        }

        private async Task<List<SpotifyPlaylistQueryResult>> QuerySpotifyForPlaylistAsync(PartyGoer user, string searchQuery, int limit)
        {
            var response = await SendHttpRequestAsync(user, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(searchQuery)}&type=playlist&limit={limit}");

            response.EnsureSuccessStatusCode();

            throw new NotImplementedException();
        }

        private Task<List<ISpotifyQueryResult>> QuerySpotifyAsync(PartyGoer user, string searchQuery)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Track>> GetUserTopTracksAsync(string spotifyId, int limit = 10)
        {
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetTopTracks], $"limit={limit}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Track> recommendedTrackUris = new List<Track>();
            foreach (var item in json["items"])
            {
                recommendedTrackUris.Add(new Track
                {
                    Name = item["name"].ToString(),
                    Artist = item["artists"].First()["name"].ToString(),
                    Uri = item["id"].ToString(),
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
                recommendedTrackUris.Add(item["id"].ToString());
            }

            return recommendedTrackUris;
        }

        public async Task<List<Track>> GetRecommendedSongsAsync(string spotifyId, List<string> seedTrackIds, float minimumEnergy)
        {
            if (seedTrackIds.Count > 5)
                throw new ArgumentException("Seed tracks cannot exeed 5");

            if (minimumEnergy > 1 && minimumEnergy < 0)
                throw new ArgumentException("Minimum Energy must be between 0 and 1");

            seedTrackIds = seedTrackIds.Select(p => p.Replace("spotify:track:", "")).ToList();
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetRecommendedTracks], $"seed_tracks={HttpUtility.UrlEncode(ConvertToCommaDelimitedString(seedTrackIds))}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Track> recommendedSongs = new List<Track>();
            foreach (var item in json["tracks"])
            {
                recommendedSongs.Add(new Track
                {
                    Uri = item["id"].ToString(),
                    Name = item["name"].ToString(),
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

        public async Task<PartyGoerDetails> RequestAccessAndRefreshTokenFromSpotifyAsync(string code)
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

                PartyGoerDetails details = await GetCurrentUserIdAsync(accessToken);

                await _spotifyAuthentication.AddAuthenticatedPartyGoerAsync(details.Id, accessToken,
                accessTokenBody["refresh_token"].ToString(),
                Convert.ToInt32(accessTokenBody["expires_in"])
                );

                return details;
            }

            return null;
        }

        private async Task<PartyGoerDetails> GetCurrentUserIdAsync(string accessToken)
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

            return new PartyGoerDetails
            {
                Id = currentUser["id"].ToString(),
                ShouldFilterExplicitSongs = currentUser["explicit_content"]["filter_enabled"].Value<bool>(),
            };
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
            HttpResponseMessage response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetTopTracks], $"limit={count}", true);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<string> trackUris = new List<string>();

            foreach (var item in json["items"])
            {
                trackUris.Add(item["id"].ToString());
            }

            return trackUris;
        }

        public async Task<List<Track>> SearchSpotifyAsync(string spotifyId, string query)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.SearchSpotify], $"q={HttpUtility.UrlEncode(query)}&type={HttpUtility.UrlEncode("track,artist")}&limit=10", true);

            response.EnsureSuccessStatusCode();

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Track> songs = new List<Track>();

            foreach (var item in json["tracks"]["items"])
            {
                songs.Add(new Track
                {
                    Artist = item["artists"].First()["name"].ToString(),
                    AlbumImageUrl = item["album"]["images"].First()["url"].ToString(),
                    Length = item["duration_ms"].Value<int>(),
                    Name = item["name"].ToString(),
                    Uri = item["uri"].ToString()
                });
            }

            return songs;
        }

        public async Task<Domain.Errors.ServiceResult<UpdateSongError>> UpdateSongForPartyGoerAsync(PartyGoer partyGoer, List<string> songUris, int currentSongProgressInMs)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.PlaySong], new ApiParameters
            {
                Parameters = new Dictionary<string, object>
                {
                    {"device_id", _partyGoerSettingsService.GetConfigurationSetting(partyGoer).PerferredDeviceId }
                }
            }, new StartUserPlaybackSong { uris = songUris.Select(song => song.Contains("spotify:track:") ? song : $"spotify:track:{song}").ToList(), position_ms = currentSongProgressInMs });

            Domain.Errors.ServiceResult<UpdateSongError> error = new Domain.Errors.ServiceResult<UpdateSongError>();

            if (response.IsSuccessStatusCode)
            {
                return error;
            }
            else
            {
                await _logService.LogExceptionAsync(new Exception($"Unable to update song for {partyGoer.Id}"), await response.Content.ReadAsStringAsync());
                // TODO: Check status codes and add specific messaging for status codes based on Spotifys API
                error.AddError(new UpdateSongError($"Unable to update song for {partyGoer.Id}"));
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

                PartyGoerDetails details = await GetCurrentUserIdAsync(accessToken);

                await _spotifyAuthentication.RefreshAccessTokenForPartyGoerAsync(details.Id, accessToken,
                Convert.ToInt32(accessTokenBody["expires_in"])
                );
            }

        }

        private async Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false)
        {
            return await SendHttpRequestAsync(user.Id, spotifyEndpoint, content, useQueryString);
        }

        private async Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, ApiParameters queryStringParameters, object requestBodyParameters)
        {
            await RefreshTokenForUserAsync(user.Id);

            using (var requestMessage = new HttpRequestMessage(spotifyEndpoint.HttpMethod, spotifyEndpoint.EndpointUrl + AddQueryStrings(queryStringParameters)))
            {
                requestMessage.Headers.Authorization = await _spotifyAuthentication.GetAuthenticationHeaderForPartyGoerAsync(user.Id);

                if (requestBodyParameters != null)
                {
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestBodyParameters));

                }

                return await _httpClient.SendAsync(requestMessage);
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

        private string AddQueryStrings(ApiParameters queryStringParameters)
        {
            if (queryStringParameters?.Parameters == null)
            {
                return string.Empty;
            }

            int count = queryStringParameters.Parameters.Count;
            if (count > 0)
            {
                StringBuilder builder = new StringBuilder(35);

                builder.Append("?");
                int i = 0;
                foreach (KeyValuePair<string, object> parameter in queryStringParameters.Parameters)
                {
                    builder.Append($"{parameter.Key}={parameter.Value}");
                    AppendAmpersandConditonallyBetweenParameters(builder, i, count);
                    i++;
                }

                return builder.ToString();
            }

            return string.Empty;
        }

        private void AppendAmpersandConditonallyBetweenParameters(StringBuilder builder, int currentIndex, int totalCount)
        {
            if (currentIndex != (totalCount - 1))
            {
                builder.Append("&");
            }
        }

        public async Task<List<Device>> GetUserDevicesAsync(PartyGoer partyGoer)
        {
            HttpResponseMessage response = await SendHttpRequestAsync(partyGoer.Id, _apiEndpoints[ApiEndpointType.GetUserDevices]);

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

                List<Device> devices = new List<Device>();
                foreach (var item in json["devices"])
                {
                    devices.Add(new Device
                    {
                        Name = item["name"].ToString(),
                        Active = item["is_active"].Value<bool>(),
                        Id = item["id"].ToString()
                    });
                }

                return devices;

            }
            else
            {
                SpotifyApiException exception = new SpotifyApiException("Unable to get users devices from Spotify");

                await _logService.LogExceptionAsync(exception, await response.Content.ReadAsStringAsync());

                throw exception;
            }
        }
    }



    public class ApiParameters
    {
        public Dictionary<string, object> Parameters { get; set; }
    }

}
