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

        public Task<List<Song>> GetRecommendedSongsAsync(string partyGoerId, int count = 10)
        {
            return _spotifyHttpClient.GetUserTopTracksAsync(partyGoerId, count);
        }

        public async Task<string> GetUsersActiveDeviceAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetUsersActiveDeviceAsync(partyGoerId);
        }

        public async Task<List<Song>> SearchSpotifyForSongs(string partyGoerId, string query)
        {
            return await _spotifyHttpClient.SearchSpotifyAsync(partyGoerId, query);
        }
    }
}
