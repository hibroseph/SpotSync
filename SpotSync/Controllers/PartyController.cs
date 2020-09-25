using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SpotSync.Classes.Hubs;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using SpotSync.Models.Party;

namespace SpotSync.Controllers
{
    public class PartyController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IHubContext<PartyHub> _partyHubContext;
        private readonly ILogService _logService;

        public PartyController(IPartyService partyService, IHubContext<PartyHub> hubContext, ILogService logService)
        {
            _partyService = partyService;
            _partyHubContext = hubContext;
            _logService = logService;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            PartyModel model = new PartyModel();

            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.IsUserPartyingAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                model.IsCurrentlyJoinedInAParty = true;
                model.CurrentlyActiveParty = TranslateDomainPartyToPartyModel(party);

            }
            else if (await _partyService.IsUserHostingAPartyAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithHostAsync(user);

                model.IsCurrentlyHostingParty = true;
                model.CurrentlyActiveParty = TranslateDomainPartyToPartyModel(party);
            }

            return View(model);
        }

        private Models.Party.Party TranslateDomainPartyToPartyModel(Domain.Party party)
        {
            return new Models.Party.Party
            {
                PartyCode = party.PartyCode,
                Attendees = party.Attendees.Select(p => p.Id).ToList()
            };
        }

        [Authorize]
        public async Task<IActionResult> CreateParty()
        {
            PartyGoer host = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
            // See if the current user doesn't have any parties
            if (await _partyService.IsUserHostingAPartyAsync(host))
            {
                return BadRequest("You have already created a party. End the current one to create another.");
            }

            string partyCode = await _partyService.StartNewPartyAsync(host);
            await _logService.LogUserActivityAsync(host.Id, $"Created a party with code {partyCode}");

            return Json(new PartyCodeDTO { PartyCode = partyCode });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> JoinParty([FromBody]PartyCodeDTO partyCode)
        {
            if (partyCode == null)
            {
                return BadRequest("The party code was empty");
            }

            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await _partyService.JoinPartyAsync(partyCode, user))
            {
                await _logService.LogUserActivityAsync(user.Id, $"Joined a party with code {partyCode.PartyCode}");
                return Ok();
            }
            else
            {
                await _logService.LogUserActivityAsync(user.Id, $"Failed to join party with code {partyCode.PartyCode}");
                return BadRequest($"Unable to join party {partyCode.PartyCode}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LeaveParty([FromBody]PartyCodeDTO partyCode)
        {
            if (partyCode == null)
            {
                return BadRequest("The party code was empty");
            }

            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!await _partyService.LeavePartyAsync(user))
            {
                await _logService.LogUserActivityAsync(user.Id, $"User failed to leave party {partyCode.PartyCode}");
                return BadRequest($"You are currently not joined with party: {partyCode.PartyCode}");
            }

            await _logService.LogUserActivityAsync(user.Id, $"User successfully left party {partyCode.PartyCode}");

            return Ok();
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
                Domain.Party party = await _partyService.GetPartyWithHostAsync(user);

                await _logService.LogUserActivityAsync(user.Id, $"User updated song for party with code {partyCode.PartyCode}");
                return await UpdateCurrentSongForEveryoneInPartyAsync(party, user);
            }
            else if (await _partyService.IsUserPartyingAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithAttendeeAsync(user);

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
        [HttpPost]
        public async Task<IActionResult> UpdateQueueForParty([FromBody]PartyCodeDTO partyCode)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Song> playlist = null;

            if (await _partyService.IsUserHostingAPartyAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithHostAsync(user);

                playlist = await UpdatePlaylistForEveryoneInPartyAsync(party, user);

                await party.StartPlaylistAsync();

                // update the playlist for everyone
                await _partyHubContext.Clients.Group(party.PartyCode).SendAsync("UpdatePlaylist", playlist, playlist.First());
            }
            else if (await _partyService.IsUserPartyingAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                playlist = await UpdatePlaylistForEveryoneInPartyAsync(party, user);

                await party.StartPlaylistAsync();

                // update the playlist for everyone
                await _partyHubContext.Clients.Group(party.PartyCode).SendAsync("UpdatePlaylist", playlist, playlist.First());
            }

            if (playlist != null)
            {
                await _logService.LogUserActivityAsync(user.Id, $"User successfully updated playlist for party {partyCode.PartyCode}");
                return Ok();
            }
            else
            {
                await _logService.LogUserActivityAsync(user.Id, $"User failed to update playlist for party with code {partyCode.PartyCode}");
                return BadRequest("Unable to create a new playlist");
            }
        }

        private List<Song> CleansePlaylist(List<Song> playlist)
        {
            var newPlaylist = playlist.ToList();
            return newPlaylist.Select(song => { song.TrackUri = song.TrackUri.Substring(14); return song; }).ToList();
        }

        private async Task<List<Song>> UpdatePlaylistForEveryoneInPartyAsync(Domain.Party party, PartyGoer partyGoer)
        {
            return await _partyService.CreatePartyPlaylistForEveryoneInPartyAsync(party, partyGoer);
        }

        private async Task<IActionResult> UpdateCurrentSongForEveryoneInPartyAsync(Domain.Party party, PartyGoer partyGoer)
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