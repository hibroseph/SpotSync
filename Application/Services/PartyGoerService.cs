using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class PartyGoerService : IPartyGoerService
    {
        ISpotifyHttpClient _spotifyHttpClient;
        public PartyGoerService(ISpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }
        public async Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetCurrentSongAsync(partyGoerId);
        }
    }
}
