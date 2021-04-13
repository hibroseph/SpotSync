using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotSync.Application.Authentication
{
    public class SpotifyAuthentication : ISpotifyAuthentication
    {
        public SpotifyAuthentication(string clientId, string clientSecret, string redirectUrl)
        {
            ClientId = clientId;
            RedirectUrl = redirectUrl;
            ClientSecret = clientSecret;

            AuthenticationTokens = new Dictionary<string, AuthenticationToken>();
        }
        public string ClientId { get; }
        public string RedirectUrl { get; }
        public string ClientSecret { get; }
        private List<string> Scopes { get; }
        private Dictionary<string, AuthenticationToken> AuthenticationTokens { get; }

        public Task RemoveAuthenticatedPartyGoerAsync(string partyGoerId)
        {
            try
            {
                AuthenticationTokens.Remove(partyGoerId);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }
        public Task AddAuthenticatedPartyGoerAsync(string partyGoerId, string accessToken, string refreshToken, int secondsTillAccessTokenExpires)
        {
            try
            {
                if (!AuthenticationTokens.ContainsKey(partyGoerId))
                {
                    AuthenticationTokens.Add(partyGoerId, new AuthenticationToken(accessToken, refreshToken, secondsTillAccessTokenExpires));
                }
                else
                {
                    AuthenticationTokens[partyGoerId] = new AuthenticationToken(accessToken, refreshToken, secondsTillAccessTokenExpires);
                }

                return Task.CompletedTask;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Unsuccessfully added authenticated party goer");
            }
        }

        public Task<AuthenticationHeaderValue> GetAuthenticationHeaderForPartyGoerAsync(string partyGoerId)
        {
            AuthenticationToken token;

            if (!AuthenticationTokens.TryGetValue(partyGoerId, out token))
            {
                throw new Exception($"There was no authentication token associated with party goer {partyGoerId}");
            }

            return Task.FromResult(new AuthenticationHeaderValue("Bearer", token.AccessToken));
        }

        public Task<bool> DoesAccessTokenNeedRefreshAsync(string partyGoerId)
        {
            return Task.FromResult(AuthenticationTokens[partyGoerId].IsAccessTokenExpired());
        }

        public string GetRefreshTokenForPartyGoer(string partyGoerId)
        {
            return AuthenticationTokens[partyGoerId].RefreshToken;
        }

        public Task RefreshAccessTokenForPartyGoerAsync(string partyGoerId, string accessToken, int expiresInXSeconds)
        {
            AuthenticationTokens[partyGoerId].UpdateAccessToken(accessToken, expiresInXSeconds);

            return Task.CompletedTask;
        }

        public Task<string> GetAccessTokenAsync(PartyGoer partyGoer)
        {
            return Task.FromResult(AuthenticationTokens[partyGoer.GetId()].AccessToken);
        }
    }
}
