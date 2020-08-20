using System;
using System.Collections.Generic;
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

        public Task AddAuthenticatedPartyGoerAsync(string partyGoerId, string accessToken, string refreshToken)
        {
            try
            {
                AuthenticationTokens.Add(partyGoerId, new AuthenticationToken(accessToken, refreshToken));

                return Task.CompletedTask;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Unsuccessfully added authenticated party goer");
            }
        }

        public Task<string> GetAccessTokenForPartyGoerAsync(string partyGoerId)
        {
            // TODO: Add logic to check to see if the token is expired and if it is, refresh it automatically
            AuthenticationToken token;
            if (!AuthenticationTokens.TryGetValue(partyGoerId, out token))
            {
                throw new Exception($"There was no authentication token associated with party goer {partyGoerId}");
            }

            return Task.FromResult(token.AccessToken);
        }

        public Task<bool> DoesAccessTokenExistAsync(string accessToken)
        {
            foreach (KeyValuePair<string, AuthenticationToken> entry in AuthenticationTokens)
            {
                if (entry.Value.AccessToken.Equals(accessToken, StringComparison.Ordinal))
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
