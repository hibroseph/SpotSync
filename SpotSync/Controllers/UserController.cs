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

namespace SpotSync.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IPartyGoerService _partyGoerService;
        private ILogService _logService;
        public UserController(IPartyGoerService partyGoerService, ILogService logService)
        {
            _partyGoerService = partyGoerService;
            _logService = logService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SuggestedSongs(int limit = 5)
        {
            List<Song> recommendedSongs = await _partyGoerService.GetRecommendedSongsAsync(_partyGoerService.GetCurrentPartyGoer().Id);

            return new JsonResult(recommendedSongs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckSpotifyForConnection()
        {
            string deviceName = await _partyGoerService.GetUsersActiveDeviceAsync(_partyGoerService.GetCurrentPartyGoer().Id);

            return new JsonResult(new { DeviceName = deviceName });
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

                var queryResults = await _partyGoerService.SearchSpotifyAsync(query, queryType ?? SpotifyQueryType.All);

                return new JsonResult(queryResults);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"Error occured while trying to query Spotify. Search query: {query} queryType: {queryType}");
                return StatusCode(500);
            }
        }
    }
}