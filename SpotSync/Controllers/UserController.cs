using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using SpotSync.Models.Party;
using SpotSync.Domain.Contracts;

namespace SpotSync.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IPartyGoerService _partyGoerService;
        private ILogService _logService;
        private ISpotifyAuthentication _spotifyAuthentication;
        public UserController(IPartyGoerService partyGoerService, ILogService logService, ISpotifyAuthentication spotifyAuthentication)
        {
            _partyGoerService = partyGoerService;
            _logService = logService;
            _spotifyAuthentication = spotifyAuthentication;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SuggestedSongs(int limit = 5)
        {
            List<Track> recommendedSongs = await _partyGoerService.GetRecommendedSongsAsync((await _partyGoerService.GetCurrentPartyGoerAsync()).Id);

            return new JsonResult(recommendedSongs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckSpotifyForConnection()
        {
            string deviceName = await _partyGoerService.GetUsersActiveDeviceAsync((await _partyGoerService.GetCurrentPartyGoerAsync()).Id);

            return new JsonResult(new { DeviceName = deviceName });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPartyGoerSpotifyAccessToken()
        {
            return new JsonResult(new { AccessToken = await _spotifyAuthentication.GetAccessTokenAsync(await _partyGoerService.GetCurrentPartyGoerAsync()) });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchSpotify(string query, SpotifyQueryType? queryType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return StatusCode(200);
                }

                return new JsonResult(await _partyGoerService.SearchSpotifyAsync(query, queryType ?? SpotifyQueryType.All));
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"Error occured while trying to query Spotify. Search query: {query} queryType: {queryType}");
                return StatusCode(500);
            }
        }
    }
}