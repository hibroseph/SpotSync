using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        ISpotifyHttpClient _spotifyHttpClient;
        ISpotifyAuthentication _spotifyAuthentication;

        public AuthenticationService(ISpotifyHttpClient spotifyHttpClient, ISpotifyAuthentication spotifyAuthentication)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _spotifyAuthentication = spotifyAuthentication;
        }

        public async Task<PartyGoerDetails> AuthenticateUserWithAccessCodeAsync(string code)
        {
            return await _spotifyHttpClient.RequestAccessAndRefreshTokenFromSpotifyAsync(code);
        }

        public async Task LogOutUserAsync(string userId)
        {
            await _spotifyAuthentication.RemoveAuthenticatedPartyGoerAsync(userId);
        }


    }
}
