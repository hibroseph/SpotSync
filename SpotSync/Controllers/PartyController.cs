using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpotSync.Classes.Hubs;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using SpotSync.Models.Dashboard;
using SpotSync.Models.Party;
using SpotSync.Models.Shared;
using Track = SpotSync.Domain.Track;

namespace SpotSync.Controllers
{
    public class PartyController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IPartyGoerService _partyGoerService;
        private readonly IHubContext<PartyHub> _partyHubContext;
        private readonly ILogService _logService;

        public PartyController(IPartyService partyService, IHubContext<PartyHub> hubContext, ILogService logService, IPartyGoerService partyGoerService)
        {
            _partyService = partyService;
            _partyGoerService = partyGoerService;
            _partyHubContext = hubContext;
            _logService = logService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> StartParty(BaseModel<DashboardModel> model)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.GetPartyWithAttendeeAsync(user) != null)
            {
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "Cannot create a party when you are joined in one. You need to leave the party you are currently in" });
            }

            //List<string> seedTrackUris = model.PageModel.SuggestedSongs.Where(p => p.Selected).Select(p => p.TrackUri).Take(5).ToList();

            //string partyCode = await _partyService.StartPartyWithSeedSongsAsync(seedTrackUris, user);

            string partyCode = await _partyService.StartPartyAsync();

            return RedirectToAction("Index", new { PartyCode = partyCode });
        }

        [Authorize]
        public async Task<IActionResult> TogglePlaybackState(string partyCode)
        {
            try
            {
                await _partyService.TogglePlaybackStateAsync(partyCode, await _partyGoerService.GetCurrentPartyGoerAsync());
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in TogglePlaybackState()");
                return new StatusCodeResult(500);
            }

            return new StatusCodeResult(200);
        }


        [Authorize]
        public async Task<IActionResult> Index(string partyCode)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Party party;

            if (string.IsNullOrWhiteSpace(partyCode))
            {
                party = await _partyService.GetPartyWithAttendeeAsync(user);
            }
            else
            {
                party = await _partyService.GetPartyWithCodeAsync(partyCode);
            }

            if (party == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            List<Track> usersSuggestedSongs = null;

            bool isUserListening = party.IsListener(user);

            if (isUserListening)
            {
                usersSuggestedSongs = await _partyGoerService.GetRecommendedSongsAsync(user.Id);
            }

            PartyModel model = new PartyModel
            {
                PartyCode = party.GetPartyCode(),
                SuggestedSongs = usersSuggestedSongs?.Select(song => ConvertDomainSongToModelSong(song)).ToList(),
                IsUserListening = isUserListening
            };

            BaseModel baseModel = new BaseModel(true, model.PartyCode);
            return View(new BaseModel<PartyModel>(model, baseModel));
        }

        private SongModel ConvertDomainSongToModelSong(Track song)
        {
            return new SongModel
            {
                Title = song.Name,
                Artist = song.Artist,
                AlbumImageUrl = song.AlbumImageUrl,
                Length = song.Length,
                TrackUri = song.Uri
            };
        }

        [Authorize]
        public async Task<IActionResult> JoinParty(string partyCode)
        {

            PartyCodeDTO partyCodeDto = new PartyCodeDTO
            {
                PartyCode = partyCode
            };

            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (partyCodeDto == null)
            {
                await _logService.LogUserActivityAsync(user.Id, $"Failed to join party with null party code");
                return RedirectToAction("Index", "Dashboard");
            }

            if (await _partyService.IsUserPartyingAsync(user))
            {
                // User can only join 1 party at a time
                return RedirectToAction("Index", "Dashboard", new { ErrorMessage = "You cannot join 2 parties. You must leave the first to join another" });
            }

            if (await _partyService.JoinPartyAsync(partyCodeDto, user))
            {
                //await _partyService.SyncUserWithSongAsync(user);
                await _logService.LogUserActivityAsync(user.Id, $"Joined a party with code {partyCodeDto.PartyCode}");

                return RedirectToAction("Index", "Party", new { PartyCode = partyCode });
            }
            else
            {
                await _logService.LogUserActivityAsync(user.Id, $"Failed to join party with code {partyCodeDto.PartyCode}");

                return RedirectToAction("Index", "Dashboard", new { ErrorMessage = $"A party does not exist with the party code {partyCode}" });
            }
        }

        [Authorize]
        public async Task<IActionResult> LeaveParty(string partyCode)
        {
            if (partyCode == null)
            {
                RedirectToAction("Index", "Dashboard");
            }

            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!await _partyService.LeavePartyAsync(user))
            {
                await _logService.LogUserActivityAsync(user.Id, $"User failed to leave party {partyCode}");
                return RedirectToAction("Index", "Dashboard");
            }

            await _logService.LogUserActivityAsync(user.Id, $"User successfully left party {partyCode}");

            return RedirectToAction("Index", "Dashboard"); ;
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> EndParty()
        {
            PartyGoer host = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.IsUserHostingAPartyAsync(host))
            {
                if (await _partyService.EndPartyAsync(host))
                {
                    await _logService.LogUserActivityAsync(host.Id, $"User successfully ended party");
                    return Ok();
                }
                else
                {
                    await _logService.LogUserActivityAsync(host.Id, $"User failed to end party");
                    return BadRequest("There was an issue with deleting your party");
                }
            }
            else
            {
                await _logService.LogUserActivityAsync(host.Id, $"User failed to end party because they weren't the host");
                return BadRequest("Unable to delete party. You are not hosting any parties");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSongForParty([FromBody]PartyCodeDTO partyCode)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.IsUserHostingAPartyAsync(user))
            {
                Party party = await _partyService.GetPartyWithHostAsync(user);

                await _logService.LogUserActivityAsync(user.Id, $"User updated song for party with code {partyCode.PartyCode}");
                return await UpdateCurrentSongForEveryoneInPartyAsync(party, user);
            }
            else if (await _partyService.IsUserPartyingAsync(user))
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                await _logService.LogUserActivityAsync(user.Id, $"User updated song for party with code {partyCode.PartyCode}");
                return await UpdateCurrentSongForEveryoneInPartyAsync(party, user);
            }
            else
            {
                await _logService.LogUserActivityAsync(user.Id, $"User failed tp update song for party with code {partyCode.PartyCode}");
                return BadRequest($"You are currently not hosting a party or attending a party: {partyCode.PartyCode}");
            }
        }

        [Authorize]
        public async Task<IActionResult> UpdateSongForUser()
        {
            try
            {
                PartyGoer listener = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _partyService.SyncListenerWithSongAsync(listener);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in UpdateSongForUser()");
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateQueueForParty([FromBody]PartyCodeDTO partyCode)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.IsUserPartyingAsync(user))
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                await UpdatePlaylistForEveryoneInPartyAsync(party, user);

                return Ok();
            }
            else
            {
                return new StatusCodeResult(400);
            }

        }

        private List<Track> CleansePlaylist(List<Track> playlist)
        {
            var newPlaylist = playlist.ToList();
            return newPlaylist.Select(song => { song.Uri = song.Uri.Substring(14); return song; }).ToList();
        }

        private async Task UpdatePlaylistForEveryoneInPartyAsync(Party party, PartyGoer partyGoer)
        {
            await DomainEvents.RaiseAsync(new PlaylistEnded { PartyCode = party.GetPartyCode() });
        }

        private async Task<IActionResult> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer partyGoer)
        {
            var response = await _partyService.UpdateCurrentSongForEveryoneInPartyAsync(party, partyGoer);

            if (response.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(ConstructFriendlyMessage(response.Errors));
            }
        }

        private string ConstructFriendlyMessage(List<UpdateSongError> errors)
        {
            if (errors.Count == 1)
            {
                return errors.First().FriendlyMessage;
            }

            StringBuilder builder = new StringBuilder(50);

            foreach (var error in errors)
            {
                builder.Append(error.FriendlyMessage);
                builder.Append(". ");
            }

            return builder.ToString();
        }
    }
}