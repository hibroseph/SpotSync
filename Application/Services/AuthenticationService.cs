using SpotSync.Application.Authentication;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.DTO;
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
        IPartyGoerService _partyGoerService;

        public AuthenticationService(ISpotifyHttpClient spotifyHttpClient, ISpotifyAuthentication spotifyAuthentication, IPartyGoerService partyGoerService)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _spotifyAuthentication = spotifyAuthentication;
            _partyGoerService = partyGoerService;
        }

        public async Task<PartyGoer> AuthenticateUserWithAccessCodeAsync(string code)
        {
            User user = await _spotifyHttpClient.RequestAccessAndRefreshTokenFromSpotifyAsync(code);

            PartyGoer partyGoer = new PartyGoer(user.SpotifyId, user.ExplicitSettings.Filter, user.Market, user.Product);

            await _partyGoerService.LoginUser(partyGoer);

            return partyGoer;

        }

        public async Task LogOutUserAsync(string userId)
        {
            await _spotifyAuthentication.RemoveAuthenticatedPartyGoerAsync(userId);
        }


    }
}
