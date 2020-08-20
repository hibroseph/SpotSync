using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotSync.Application.Authentication
{
    public class SpotifyAuthentication
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

        public Task AddAuthenticatedPartyGoerAsync(string partyGoerId, string accessToken, string refreshToken, int secondsTillAccessTokenExpires)
        {
            try
            {
                AuthenticationTokens.Add(partyGoerId, new AuthenticationToken(accessToken, refreshToken, secondsTillAccessTokenExpires));

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

            if (token.IsAccessTokenExpired())
            {
                // TODO: Add logic to refresh current token

            }

            return Task.FromResult(new AuthenticationHeaderValue("Bearer", token.AccessToken));
        }

    }
}
