using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotSync.Infrastructure
{
    public class SpotifyHttpClient : ISpotifyHttpClient
    {
        private HttpClient _httpClient;
        private SpotifyAuthentication _spotifyAuthentication;
        private Dictionary<SpotifyApiEndpointType, string> _spotifyApiEndpoints;

        public SpotifyHttpClient(SpotifyAuthentication spotifyAuthentication)
        {
            _httpClient = new HttpClient();
            _spotifyAuthentication = spotifyAuthentication;
            _spotifyApiEndpoints = new Dictionary<SpotifyApiEndpointType, string>
            {
                { SpotifyApiEndpointType.CurrentSong, "https://api.spotify.com/v1/me/player/currently-playing" },
                { SpotifyApiEndpointType.PlaySong, "https://api.spotify.com/v1/me/player/play" },
                { SpotifyApiEndpointType.Token, "https://accounts.spotify.com/api/token" },
                { SpotifyApiEndpointType.UserInformation, "https://api.spotify.com/v1/me" }
            };
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

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, _spotifyApiEndpoints[SpotifyApiEndpointType.Token]))
            {
                requestMessage.Content = new FormUrlEncodedContent(properties);

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response is null)
            {
                throw new Exception("The response from requesting the access and refresh token was null");
            }

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

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _spotifyApiEndpoints[SpotifyApiEndpointType.UserInformation]))
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
            await RefreshTokenForUserAsync(partyGoerId);

            HttpResponseMessage response;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _spotifyApiEndpoints[SpotifyApiEndpointType.CurrentSong]))
            {
                requestMessage.Headers.Authorization = await _spotifyAuthentication.GetAuthenticationHeaderForPartyGoerAsync(partyGoerId);

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            JObject currentSong = JObject.Parse(responseContent);

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

        public async Task<bool> UpdateSongForPartyGoerAsync(string partyGoerId, CurrentSongDTO currentSong)
        {
            await RefreshTokenForUserAsync(partyGoerId);

            HttpResponseMessage response;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, _spotifyApiEndpoints[SpotifyApiEndpointType.PlaySong]))
            {
                requestMessage.Headers.Authorization = await _spotifyAuthentication.GetAuthenticationHeaderForPartyGoerAsync(partyGoerId);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(new StartUserPlaybackSong
                {
                    uris = new List<string> { currentSong.TrackUri },
                    position_ms = currentSong.ProgressMs
                }));

                response = await _httpClient.SendAsync(requestMessage);
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
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

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, _spotifyApiEndpoints[SpotifyApiEndpointType.Token]))
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
    }
}
