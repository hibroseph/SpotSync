using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Json;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;
using SpotifyModels = SpotSync.Domain.Contracts.SpotifyApi.Models;

namespace SpotSync.Infrastructure.SpotifyApi
{
    public class SpotifyHttpClient : ISpotifyHttpClient
    {
        private IHttpClient _httpClient;
        private ISpotifyAuthentication _spotifyAuthentication;
        private ILogService _logService;
        private Dictionary<ApiEndpointType, SpotifyEndpoint> _apiEndpoints;
        private SpotifyToSpotibroModelMapper _mapper;

        public SpotifyHttpClient(ISpotifyAuthentication spotifyAuthentication, IHttpClient httpClient, ILogService logService)
        {
            _httpClient = httpClient;
            _spotifyAuthentication = spotifyAuthentication;
            _logService = logService;

            _mapper = new SpotifyToSpotibroModelMapper();

            // Once this class gets refactored this should move out of here
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
                { ApiEndpointType.PausePlayback, new SpotifyEndpoint {EndpointUrl = "https://api.spotify.com/v1/me/player/pause", HttpMethod = HttpMethod.Put } },
                { ApiEndpointType.GetUserPlaylists, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/me/playlists", HttpMethod = HttpMethod.Get } },
                { ApiEndpointType.GetPlaylistItems, new SpotifyEndpoint { EndpointUrl = "https://api.spotify.com/v1/playlists/{playlist_id}/tracks", HttpMethod = HttpMethod.Get,
                Keys = new List<string> {"{playlist_id}"} } },
                { ApiEndpointType.ArtistInformation, new SpotifyEndpoint { EndpointUrl =  "https://api.spotify.com/v1/artists/{id}", HttpMethod = HttpMethod.Get,
                Keys = new List<string> { "{id}"} } }, {ApiEndpointType.ArtistTopTracks, new SpotifyEndpoint{ EndpointUrl = "https://api.spotify.com/v1/artists/{id}/top-tracks", HttpMethod = HttpMethod.Get,
                Keys = new List<string> {"{id}" }  } }
            };
        }

        public async Task<SpotibroModels.ArtistInformation> GetArtistInformationAsync(PartyGoer partyGoer, string artistId)
        {
            var artistInformationTask = GetBasicArtistInformationAsync(partyGoer, artistId);
            var artistTopTracksTask = GetTopTracksAsync(partyGoer, artistId);

            await Task.WhenAll(artistInformationTask, artistTopTracksTask);

            return _mapper.Convert(artistInformationTask.Result, artistTopTracksTask.Result);
        }

        private async Task<Domain.Contracts.SpotifyApi.Models.ArtistInformation> GetBasicArtistInformationAsync(PartyGoer partyGoer, string artistId)
        {
            var parameters = new ApiParameters { Keys = new Dictionary<string, string> { { "{id}", artistId } } };
            var artistResponse = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.ArtistInformation], parameters);

            return await artistResponse.Content.ReadFromJsonAsync<Domain.Contracts.SpotifyApi.Models.ArtistInformation>();
        }

        private string CleanTrackId(string trackUri)
        {
            return trackUri.Replace("spotify:track:", string.Empty).Split("+").First();

        }

        private async Task<TopTracks> GetTopTracksAsync(PartyGoer partyGoer, string artistId)
        {
            ApiParameters parameters = new ApiParameters { Keys = new Dictionary<string, string> { { "{id}", artistId } }, Parameters = new Dictionary<string, string> { { "market", partyGoer.GetMarket() } } };
            var artistTopTracksResponse = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.ArtistTopTracks], parameters);

            return await artistTopTracksResponse.Content.ReadFromJsonAsync<TopTracks>();
        }

        public async Task TogglePlaybackAsync(PartyGoer partyGoer, PlaybackState state)
        {
            var response = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.PausePlayback]);

            EnsureSuccessfulResponse(response);
        }

        public async Task<User> GetUserDetailsAsync(string spotifyId)
        {
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.UserInformation]);

            EnsureSuccessfulResponse(response);

            return await response.Content.ReadFromJsonAsync<User>();

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

            var searchResults = await response.Content.ReadFromJsonAsync<SearchResults>();
            return searchResults.Tracks.Items.Select(p => new SpotifyTrackQueryResult
            {
                Id = CleanTrackId(p.Id),
                Artists = p.Artists.Select(p => new Artist { Id = p.Id, Name = p.Name }).ToList(),
                Duration = p.Duration,
                Name = p.Name,
                Explicit = p.Explicit
            });
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
            catch (Exception)
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
                if (item != null)
                {
                    artists.Add(new SpotifyArtistQueryResult
                    {
                        Name = item["name"].ToString(),
                        Uri = item["uri"].ToString()
                    });
                }
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
                if (item != null)
                {
                    results.Add(new SpotifyAlbumQueryResult
                    {
                        Artist = item["artists"].First()["name"].ToString(),
                        Name = item["name"].ToString(),
                        Uri = item["uri"].ToString()
                    });
                }
            }

            return results;
        }

        public async Task<List<SpotibroModels.Track>> GetUserTopTracksAsync(string spotifyId, int limit = 10)
        {
            var response = await SendHttpRequestAsync(spotifyId, _apiEndpoints[ApiEndpointType.GetTopTracks], $"limit={limit}", true);

            response.EnsureSuccessStatusCode();

            return _mapper.Convert(await response.Content.ReadFromJsonAsync<PagedObject<SpotifyModels.Track>>());

        }

        public async Task<List<Domain.Track>> GetRecommendedTracksAsync(PartyGoer partyGoer, RecommendedTracksSeed recommendedTrackSeeds)
        {
            List<string> seedTrackUris = recommendedTrackSeeds?.SeedTrackUris;
            List<string> seedArtistUris = recommendedTrackSeeds?.SeedArtistUris;

            if (seedTrackUris?.Count + seedArtistUris?.Count > 5 || seedTrackUris?.Count + seedArtistUris?.Count == 0)
            {
                throw new ArgumentException("The count of seeds need to be between 1 and 5");
            }

            recommendedTrackSeeds.SeedTrackUris = seedTrackUris.Select(p => p.Replace("spotify:track:", "").Split("+").First()).ToList();

            var response = await SendHttpRequestAsync(partyGoer.GetSpotifyId(), _apiEndpoints[ApiEndpointType.GetRecommendedTracks], AddRecommendedSeedApiParmeters(recommendedTrackSeeds), true);

            RecommendedTracks tracks = await response.Content.ReadFromJsonAsync<RecommendedTracks>();

            List<Domain.Track> recommendedSongs = new List<Domain.Track>();

            foreach (SpotifyModels.Track track in tracks.Tracks)
            {
                if (track.Markets.Contains(partyGoer.GetMarket()) && track != null)
                {
                    recommendedSongs.Add(new Domain.Track
                    {
                        AlbumImageUrl = null,
                        Artists = track.Artists.Select(p => new SpotifyModels.Artist { Id = p.Id, Name = p.Name }).ToList(),
                        Explicit = track.Explicit,
                        Length = track.Duration,
                        Name = track.Name,
                        Id = track.Id
                    });
                }
            }

            return recommendedSongs.ToList();
        }

        private string AddRecommendedSeedApiParmeters(RecommendedTracksSeed getRecommendedSongs)
        {
            return $"{(!getRecommendedSongs.SeedTrackUris.IsNullOrEmpty() ? $"seed_tracks={ HttpUtility.UrlEncode(ConvertToCommaDelimitedString(getRecommendedSongs.SeedTrackUris))}&" : string.Empty)}{(!getRecommendedSongs.SeedArtistUris.IsNullOrEmpty() ? $"seed_artists={ HttpUtility.UrlEncode(ConvertToCommaDelimitedString(getRecommendedSongs.SeedArtistUris))}&" : string.Empty)}{(!string.IsNullOrWhiteSpace(getRecommendedSongs.Market) ? $"{getRecommendedSongs.Market}" : string.Empty)}";


        }

        private string ConvertToCommaDelimitedString(List<string> items)
        {
            return string.Join(",", items);
        }

        public async Task<User> RequestAccessAndRefreshTokenFromSpotifyAsync(string code)
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

                User user = await GetSpotifyUserWithAccessToken(accessToken);

                await _spotifyAuthentication.AddAuthenticatedPartyGoerAsync(user.SpotifyId, accessToken,
                accessTokenBody["refresh_token"].ToString(),
                Convert.ToInt32(accessTokenBody["expires_in"])
                );

                return user;
            }

            return null;
        }

        private async Task<User> GetSpotifyUserWithAccessToken(string accessToken)
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

            return await response.Content.ReadFromJsonAsync<User>();
        }

        private bool ConvertToHasPremiumSpotify(string spotifyProduct)
        {
            return spotifyProduct.Contains("premium", StringComparison.OrdinalIgnoreCase);
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

        public async Task<Domain.Errors.ServiceResult<UpdateSongError>> UpdateSongForPartyGoerAsync(PartyGoer partyGoer, List<string> songUris, int currentSongProgressInMs)
        {
            string perferredDeviceId = partyGoer.GetPerferredDeviceId();

            ApiParameters parameters = null;

            if (!string.IsNullOrWhiteSpace(perferredDeviceId))
            {
                parameters = new ApiParameters
                {
                    Parameters = new Dictionary<string, string>
                {
                    {"device_id", perferredDeviceId }

                }
                };
            }

            HttpResponseMessage response = null;

            if (parameters != null)
            {
                response = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.PlaySong], parameters,
                new StartUserPlaybackSong { uris = songUris.Select(song => song.Contains("spotify:track:") ? song : $"spotify:track:{song}".Split('+').First()).ToList(), position_ms = currentSongProgressInMs });
            }
            else
            {
                response = await SendHttpRequestAsync(partyGoer, _apiEndpoints[ApiEndpointType.PlaySong],
                new StartUserPlaybackSong { uris = songUris.Select(song => song.Contains("spotify:track:") ? song : $"spotify:track:{song}".Split('+').First()).ToList(), position_ms = currentSongProgressInMs });
            }

            Domain.Errors.ServiceResult<UpdateSongError> error = new Domain.Errors.ServiceResult<UpdateSongError>();

            if (response.IsSuccessStatusCode)
            {
                return error;
            }
            else
            {
                await _logService.LogExceptionAsync(new Exception($"Unable to update song for {partyGoer.GetId()}"), await response.Content.ReadAsStringAsync());
                // TODO: Check status codes and add specific messaging for status codes based on Spotifys API
                error.AddError(new UpdateSongError($"Unable to update song for {partyGoer.GetId()}"));
                return error;
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

        public async Task RefreshTokenForUserAsync(string partyGoerId)
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

                User user = await GetSpotifyUserWithAccessToken(accessToken);

                await _spotifyAuthentication.RefreshAccessTokenForPartyGoerAsync(user.SpotifyId, accessToken,
                Convert.ToInt32(accessTokenBody["expires_in"])
                );
            }

        }


        public async Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint endpoint, ApiParameters parameters)
        {
            return await SendHttpRequestAsync(user, endpoint, parameters, null);
        }
        public async Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false)
        {
            return await SendHttpRequestAsync(user.GetId(), spotifyEndpoint, content, useQueryString);
        }

        public async Task<HttpResponseMessage> SendHttpRequestAsync(PartyGoer user, SpotifyEndpoint spotifyEndpoint, ApiParameters queryStringParameters, object requestBodyParameters)
        {
            await RefreshTokenForUserAsync(user.GetId());

            string spotifyEndpointUrl = spotifyEndpoint.EndpointUrl;

            // look to see if spotifyendpoint has keys
            if (spotifyEndpoint?.Keys != null)
            {
                // we need to verify these keys exist in api parameters
                foreach (string key in spotifyEndpoint.Keys)
                {
                    if (!queryStringParameters.Keys.ContainsKey(key))
                    {
                        throw new Exception($"Endpoint: {spotifyEndpoint} says it contains a key: {key} but it is not found in parameters");
                    }

                    spotifyEndpointUrl = spotifyEndpointUrl.Replace(key, queryStringParameters.Keys[key]);
                }
            }

            using (var requestMessage = new HttpRequestMessage(spotifyEndpoint.HttpMethod, spotifyEndpointUrl + AddQueryStrings(queryStringParameters)))
            {
                requestMessage.Headers.Authorization = await _spotifyAuthentication.GetAuthenticationHeaderForPartyGoerAsync(user.GetId());

                if (requestBodyParameters != null)
                {
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestBodyParameters));

                }

                return await _httpClient.SendAsync(requestMessage);
            }
        }
        public async Task<HttpResponseMessage> SendHttpRequestAsync(string spotifyId, SpotifyEndpoint spotifyEndpoint, object content = null, bool useQueryString = false)
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
                foreach (KeyValuePair<string, string> parameter in queryStringParameters.Parameters)
                {
                    builder.Append($"{parameter.Key}={HttpUtility.UrlEncode(parameter.Value)}");
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
            HttpResponseMessage response = await SendHttpRequestAsync(partyGoer.GetId(), _apiEndpoints[ApiEndpointType.GetUserDevices]);

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

}
