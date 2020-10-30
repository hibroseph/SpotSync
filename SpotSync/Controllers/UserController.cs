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
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Models.Party;

namespace SpotSync.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IPartyGoerService _partyGoerService;

        public UserController(IPartyGoerService partyGoerService)
        {
            _partyGoerService = partyGoerService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SuggestedSongs(int limit = 5)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Song> recommendedSongs = await _partyGoerService.GetRecommendedSongsAsync(user.Id);

            return new JsonResult(recommendedSongs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckSpotifyForConnection()
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            string deviceName = await _partyGoerService.GetUsersActiveDeviceAsync(user.Id);

            return new JsonResult(new { DeviceName = deviceName });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchSpotify(string query)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return StatusCode(200);
                }

                List<Song> songs = await _partyGoerService.SearchSpotifyForSongs(user.Id, query);

                return new JsonResult(songs.Select(song => new SongModel
                {
                    AlbumImageUrl = song.AlbumImageUrl,
                    Artist = song.Artist,
                    Length = song.Length,
                    Title = song.Title,
                    TrackUri = song.TrackUri
                }).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}