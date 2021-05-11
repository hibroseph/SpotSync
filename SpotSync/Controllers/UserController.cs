using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IPartyGoerService _partyGoerService;
        private ILogService _logService;
        private IPartyService _partyService;

        public UserController(IPartyGoerService partyGoerService, ILogService logService, IPartyService partyService)
        {
            _partyGoerService = partyGoerService;
            _logService = logService;
            _partyService = partyService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UsersPlaylists(int limit = 10, int offset = 0)
        {
            return new JsonResult(await _partyGoerService.GetUsersPlaylistsAsync(await _partyGoerService.GetCurrentPartyGoerAsync(), limit, offset));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UsersPlaylistItems(string playlistId)
        {
            if (string.IsNullOrWhiteSpace(playlistId))
            {
                return new NotFoundResult();
            }

            return new JsonResult(await _partyGoerService.GetPlaylistItemsAsync(await _partyGoerService.GetCurrentPartyGoerAsync(), playlistId));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SuggestedSongs(int limit = 5)
        {
            return new JsonResult(await _partyGoerService.GetRecommendedSongsAsync((await _partyGoerService.GetCurrentPartyGoerAsync()).GetId(), limit));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SuggestedContributions(List<string> excludedIds)
        {
            var result = await _partyGoerService.GetSuggestedContributionsAsync(await _partyGoerService.GetCurrentPartyGoerAsync(), excludedIds);

            return new JsonResult(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            PartyGoer currentUser = await _partyGoerService.GetCurrentPartyGoerAsync();

            Party party = await _partyService.GetPartyWithAttendeeAsync(currentUser);

            if (party != null)
            {
                return Ok(new { IsInParty = true, Party = new { PartyCode = party.GetPartyCode() }, Details = new { Id = currentUser.GetId() } });
            }
            else
            {
                return Ok(new { IsInParty = false, UserDetails = new { Id = currentUser.GetId() } });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckSpotifyForConnection()
        {
            string deviceName = await _partyGoerService.GetUsersActiveDeviceAsync((await _partyGoerService.GetCurrentPartyGoerAsync()).GetId());

            return new JsonResult(new { DeviceName = deviceName });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetActiveDevice()
        {
            string deviceName = await _partyGoerService.GetUsersActiveDeviceAsync((await _partyGoerService.GetCurrentPartyGoerAsync()).GetId());

            return new JsonResult(new { DeviceName = deviceName });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetActiveDevices()
        {
            ServiceResult<List<Device>> devicesResult = await _partyGoerService.GetUserDevicesAsync(await _partyGoerService.GetCurrentPartyGoerAsync());

            if (devicesResult.IsSuccessful())
            {
                return new JsonResult(devicesResult.Result);
            }
            else
            {
                return new JsonResult(new { Type = "Error", Message = "Unable to reach Spotify API, try again" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPartyGoerSpotifyAccessToken()
        {
            return new JsonResult(new { AccessToken = await _partyGoerService.GetPartyGoerAccessTokenAsync(await _partyGoerService.GetCurrentPartyGoerAsync()) });
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FavoriteTrackAsync(string trackId)
        {
            try
            {
                PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

                await _partyGoerService.FavoriteTrackAsync(user, trackId);

                return Ok();
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"Error occurred while user was trying to favorite song {trackId}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnfavoriteTrackAsync(string trackId)
        {
            try
            {
                PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

                await _partyGoerService.UnfavoriteTrackAsync(user, trackId);

                return Ok();
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"Error occurred while user was trying to unfavorite song {trackId}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavoriteTracks()
        {
            try
            {
                PartyGoer partyGoer = await _partyGoerService.GetCurrentPartyGoerAsync();
                return new JsonResult(await _partyGoerService.GetUsersFavoriteTracksAsync(partyGoer));
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in GetFavoriteTracks");
                return StatusCode(500);
            }
        }
    }
}