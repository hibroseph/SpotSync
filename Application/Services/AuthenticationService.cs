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

        public AuthenticationService(ISpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task<string> AuthenticateUserWithAccessCode(string code)
        {
            return await _spotifyHttpClient.RequestAccessAndRefreshTokenFromSpotifyAsync(code);
        }
    }
}
