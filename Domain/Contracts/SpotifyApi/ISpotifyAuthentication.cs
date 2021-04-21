using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface ISpotifyAuthentication
    {
        string ClientId { get; }
        string RedirectUrl { get; }
        string ClientSecret { get; }

        Task<string> GetAccessTokenAsync(PartyGoer partyGoer);
        Task<AuthenticationHeaderValue> GetAuthenticationHeaderForPartyGoerAsync(string partyGoerId);
        Task AddAuthenticatedPartyGoerAsync(string partyGoerId, string accessToken, string refreshToken, int secondsTillAccessTokenExpires);
        Task<bool> DoesAccessTokenNeedRefreshAsync(string partyGoerId);
        string GetRefreshTokenForPartyGoer(string partyGoerId);
        Task RefreshAccessTokenForPartyGoerAsync(string partyGoerId, string accessToken, int expiresInXSeconds);
        Task RemoveAuthenticatedPartyGoerAsync(string partyGoerId);
    }
}
