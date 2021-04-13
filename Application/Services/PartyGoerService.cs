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
using SpotSync.Domain.Errors;
using System.Collections.Concurrent;

namespace SpotSync.Application.Services
{
    public class PartyGoerService : IPartyGoerService
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private IHttpContextAccessor _httpContextAccessor;
        private ISpotifyAuthentication _spotifyAuthentication;
        private IDictionary<string, PartyGoer> _partyGoerCache;
        private ILogService _logService;

        public PartyGoerService(ISpotifyHttpClient spotifyHttpClient, IHttpContextAccessor httpContextAccessor,
        ISpotifyAuthentication spotifyAuthentication, ILogService logService)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _httpContextAccessor = httpContextAccessor;
            _spotifyAuthentication = spotifyAuthentication;
            _partyGoerCache = new ConcurrentDictionary<string, PartyGoer>();
            _logService = logService;
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

        /// <summary>
        /// Accesses the current party goer. If no party goer is associated with the current session, null is returned
        /// </summary>
        /// <returns>A party goer, if unauthenticated, null</returns>
        public async Task<PartyGoer> GetCurrentPartyGoerAsync()
        {
            string partyGoerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrWhiteSpace(partyGoerId)) return null;

            if (_partyGoerCache.ContainsKey(partyGoerId))
            {
                return _partyGoerCache[partyGoerId];
            }
            else
            {
                User user = await _spotifyHttpClient.GetUserDetailsAsync(partyGoerId);

                PartyGoer newPartyGoer = new PartyGoer(user.SpotifyId, user.ExplicitSettings.Filter, user.Market, user.Product);
                _partyGoerCache.TryAdd(partyGoerId, newPartyGoer);

                return newPartyGoer;
            }
        }


        public async Task<string> GetPartyGoerAccessTokenAsync(PartyGoer partyGoer)
        {
            if (await _spotifyAuthentication.DoesAccessTokenNeedRefreshAsync(partyGoer.GetId()))
            {
                await _spotifyHttpClient.RefreshTokenForUserAsync(partyGoer.GetId());
            }

            return await _spotifyAuthentication.GetAccessTokenAsync(partyGoer);
        }

        public async Task<SpotSync.Domain.ServiceResult<List<Device>>> GetUserDevicesAsync(PartyGoer partyGoer)
        {
            try
            {
                List<Device> devices = await _spotifyHttpClient.GetUserDevicesAsync(partyGoer);

                return new Domain.ServiceResult<List<Device>> { Result = devices };
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred while trying to get user devices");

                return new Domain.ServiceResult<List<Device>> { Error = ErrorType.SpotifyApiFailed };
            }
        }

        public async Task<List<Playlist>> GetUsersPlaylistsAsync(PartyGoer user, int limit = 10, int offset = 0)
        {
            return await _spotifyHttpClient.GetUsersPlaylistsAsync(user, limit, offset);
        }

        public async Task<List<Track>> GetPlaylistItemsAsync(PartyGoer user, string playlistId)
        {
            return await _spotifyHttpClient.GetPlaylistItemsAsync(user, playlistId);
        }
    }
}
