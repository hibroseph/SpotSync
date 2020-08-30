﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.DTO;
using SpotSync.Models.Party;

namespace SpotSync.Controllers
{
    public class PartyController : Controller
    {
        private readonly IPartyService _partyService;

        public PartyController(IPartyService partyService)
        {
            _partyService = partyService;
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
            else if (_partyService.IsUserHostingAParty(user))
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
        public IActionResult CreateParty()
        {
            PartyGoer host = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
            // See if the current user doesn't have any parties
            if (_partyService.IsUserHostingAParty(host))
            {
                return BadRequest("You have already created a party. End the current one to create another.");
            }

            string partyCode = _partyService.StartNewParty(host);
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

            if (await _partyService.JoinPartyAsync(partyCode, new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return Ok();
            }
            else
            {
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

            if (!await _partyService.LeavePartyAsync(new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return BadRequest($"You are currently not joined with party: {partyCode.PartyCode}");
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> EndParty()
        {
            PartyGoer host = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (_partyService.IsUserHostingAParty(host))
            {
                if (await _partyService.EndPartyAsync(host))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("There was an issue with deleting your party");
                }
            }
            else
            {
                return BadRequest("Unable to delete party. You are not hosting any parties");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSongForParty([FromBody]PartyCodeDTO partyCode)
        {
            PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (_partyService.IsUserHostingAParty(user))
            {
                Domain.Party party = await _partyService.GetPartyWithHostAsync(user);

                return await UpdatePlaylistForEveryoneInPartyAsync(party, user);
            }
            else if (await _partyService.IsUserPartyingAsync(user))
            {
                Domain.Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                return await UpdatePlaylistForEveryoneInPartyAsync(party, user);
            }
            else
            {
                return BadRequest($"You are currently not hosting a party or attending a party: {partyCode.PartyCode}");
            }
        }

        private async Task<IActionResult> UpdatePlaylistForEveryoneInPartyAsync(Domain.Party party, PartyGoer partyGoer)
        {
            if (await _partyService.UpdatePartyPlaylistForEveryoneInPartyAsync(party, partyGoer))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Unable to update song for everyone");
            }
        }
    }
}