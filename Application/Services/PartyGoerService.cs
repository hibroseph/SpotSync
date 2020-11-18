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
        ISpotifyHttpClient _spotifyHttpClient;
        IHttpContextAccessor _httpContextAccessor;
        public PartyGoerService(ISpotifyHttpClient spotifyHttpClient, IHttpContextAccessor httpContextAccessor)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<List<SpotifyQueryResult>> SearchSpotifyAsync(string query, SpotifyQueryType queryType, int limit = 10)
        {
            return (await _spotifyHttpClient.QuerySpotifyAsync(GetCurrentPartyGoer(), query, queryType, limit)).ToList();
        }

        public PartyGoer GetCurrentPartyGoer()
        {
            // TODO: Cache party goers since a lot of them will be made
            return new PartyGoer(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}
