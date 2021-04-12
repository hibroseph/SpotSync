using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class BrowseSpotifyService : IBrowseSpotifyService
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private ILogService _logService;

        public BrowseSpotifyService(ISpotifyHttpClient spotifyHttpClient, ILogService logService)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
        }

        public async Task<ArtistInformation> GetArtistInformationAsync(PartyGoer partyGoer, string artistId)
        {
            try
            {
                return await _spotifyHttpClient.GetArtistInformationAsync(partyGoer, artistId);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred while trying to get artist information");
                return null;
            }
        }
    }
}
