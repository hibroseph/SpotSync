using Microsoft.AspNetCore.Http;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Application.Services
{
    public class PartyGoerService : IPartyGoerService
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private IHttpContextAccessor _httpContextAccessor;
        private Dictionary<string, PartyGoer> _partyGoerCache;

        public PartyGoerService(ISpotifyHttpClient spotifyHttpClient, IHttpContextAccessor httpContextAccessor)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _httpContextAccessor = httpContextAccessor;
            _partyGoerCache = new Dictionary<string, PartyGoer>();
        }

        public async Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetCurrentSongAsync(partyGoerId);
        }

        public Task<List<Track>> GetRecommendedSongsAsync(string partyGoerId, int count = 10)
        {
            return _spotifyHttpClient.GetUserTopTracksAsync(partyGoerId, count);
        }

        public async Task<string> GetUsersActiveDeviceAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetUsersActiveDeviceAsync(partyGoerId);
        }

        public async Task<IEnumerable<ISpotifyQueryResult>> SearchSpotifyAsync(string query, SpotifyQueryType queryType, int limit = 10)
        {
            return await _spotifyHttpClient.QuerySpotifyAsync(await GetCurrentPartyGoerAsync(), query, queryType, limit);
        }

        public void SavePartyGoer(PartyGoerDetails partyGoerDetails)
        {
            if (_partyGoerCache.ContainsKey(partyGoerDetails.Id))
            {
                _partyGoerCache[partyGoerDetails.Id] = new PartyGoer(partyGoerDetails);
            }
            else
            {
                _partyGoerCache.Add(partyGoerDetails.Id, new PartyGoer(partyGoerDetails));
            }
        }

        public async Task<PartyGoer> GetCurrentPartyGoerAsync()
        {
            string partyGoerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (_partyGoerCache.ContainsKey(partyGoerId))
            {
                return _partyGoerCache[partyGoerId];
            }
            else
            {
                PartyGoerDetails partyGoerDetails = await _spotifyHttpClient.GetUserDetailsAsync(partyGoerId);

                PartyGoer newPartyGoer = new PartyGoer(partyGoerDetails);
                _partyGoerCache.Add(partyGoerId, newPartyGoer);

                return newPartyGoer;
            }
        }
    }
}
