using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IPartyGoerService
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
        Task<List<Song>> GetRecommendedSongsAsync(string partyGoerId, int count = 10);
        Task<string> GetUsersActiveDeviceAsync(string partyGoerId);
        Task<IEnumerable<ISpotifyQueryResult>> SearchSpotifyAsync(string query, SpotifyQueryType queryType, int limit = 10);
        PartyGoer GetCurrentPartyGoer();
    }
}
