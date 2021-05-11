using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Classes.Responses.Common;
using SpotSync.Classes.Responses.Party;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Events;
using SpotSync.Models.Party;
using SpotSync.Domain.PartyAggregate;
using System.Collections.Generic;
using System.Linq;

namespace SpotSync.Controllers
{
    public class PartyController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IPartyGoerService _partyGoerService;
        private readonly ILogService _logService;

        public PartyController(IPartyService partyService, ILogService logService, IPartyGoerService partyGoerService)
        {
            _partyService = partyService;
            _partyGoerService = partyGoerService;
            _logService = logService;
        }

        [Authorize]
        [HttpPost("api/[controller]/UpdateContribution")]
        public async Task<IActionResult> UpdateContribution(string partyCode, [FromBody]UpdateContributions updateContributions)
        {
            try
            {
                if (updateContributions.ContributionsToRemove == null)
                {
                    updateContributions.ContributionsToRemove = new List<UserContribution>();
                }

                if (updateContributions.NewContributions == null)
                {
                    updateContributions.NewContributions = new List<UserContribution>();
                }

                if (updateContributions != null && updateContributions.ContributionsToRemove.Count == 0 && updateContributions.NewContributions.Count == 0)
                {
                    return Ok();
                }

                PartyGoer partyGoer = await _partyGoerService.GetCurrentPartyGoerAsync();

                await _partyService.UpdateContributionsAsync(partyCode,
                updateContributions.NewContributions.Select(p => CreateContribution(partyGoer, p)).ToList(),
                updateContributions.ContributionsToRemove.Select(p => CreateContribution(partyGoer, p)).ToList());
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred while trying to add contribution");
                return StatusCode(500);
            }

            return Ok();
        }

        private Contribution CreateContribution(PartyGoer partyGoer, UserContribution userContribution)
        {
            return new Contribution
            {
                ContributedBy = partyGoer.GetSpotifyId(),
                ContributionId = Guid.NewGuid(),
                Id = userContribution.Id,
                Type = userContribution.Type,
                Name = userContribution.Name
            };
        }

        [Authorize]
        [HttpGet("api/[controller]/UserContributions")]
        public async Task<IActionResult> UserContributions(string partyCode)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();
            try
            {
                return new JsonResult(await _partyService.GetContributionsAsync(partyCode, partier));
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Failed to get user contributions");
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpDelete("api/[controller]/RemoveContribution")]
        public async Task<IActionResult> RemoveUserContribution(string partyCode, Guid contributionId)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();
            try
            {
                await _partyService.RemoveContributionAsync(partyCode, partier, contributionId);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"Failed to remove contribution {contributionId} for user {partier.GetId()}");
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpGet("api/[controller]/GetParties")]
        public async Task<IActionResult> GetParties()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> StartParty()
        {
            if (await _partyService.IsUserPartyingAsync(await _partyGoerService.GetCurrentPartyGoerAsync()))
            {
                return StatusCode(409);
            }

            string partyCode = await _partyService.StartPartyAsync();

            return Ok(new { partyCode = partyCode });
        }

        [Authorize]
        [HttpGet("api/[controller]/UsersLikesDislikes")]
        public async Task<IActionResult> GetUsersLikesDislikes(string partyCode)
        {
            try
            {
                Party party = await _partyService.GetPartyWithCodeAsync(partyCode);
                PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

                return new JsonResult(new Result<LikedDislikedSongs>(ConvertUsersLikedDislikesToDto(party.GetUsersLikesDislikes(user))));
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred while trying to get users likes and dislikes");
                return new JsonResult(new Result(false, "Unable to get your liked and disliked songs"));
            }
        }

        private LikedDislikedSongs ConvertUsersLikedDislikesToDto(LikesDislikes likedDislikedSongs)
        {
            return new LikedDislikedSongs
            {
                LikedSongs = likedDislikedSongs.GetLikedSongs(),
                DislikedSongs = likedDislikedSongs.GetDislikedSongs()
            };
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

        /// <summary>
        /// This endpoint is allowed to be accessed if you are not authenticated. If you are not authenticated, then you will be redirected to login
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> JoinParty(string partyCode)
        {
            PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

            if (user == null)
            {
                return new JsonResult(new Result(false, "User is not authenticated"));
            }


            Party party = await _partyService.GetPartyWithCodeAsync(partyCode);

            if (party == null)
            {
                await _logService.LogUserActivityAsync(user, $"Failed to join party, party code did was not a valid party: {partyCode}");

                return new JsonResult(new Result(false, "Party code was not valid"));
            }

            if (await _partyService.IsUserPartyingAsync(user))
            {
                // User can only join 1 party at a time
                return new JsonResult(new Result(false, "You cannot join 2 parties. You must leave the first to join another"));
            }

            try
            {
                party.JoinParty(user);
                await party.SyncListenerWithSongAsync(user);
                await _logService.LogUserActivityAsync(user, $"Joined a party with code {partyCode}");

                return new JsonResult(new Result<JoinedParty>(new JoinedParty { PartyCode = partyCode, SuccessfullyJoinedParty = true }));

            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, $"{user} failed to join party {partyCode}");
            }

            return new JsonResult(new Result(false, "An error occurred while trying to join party"));
        }

        [Authorize]
        public async Task<IActionResult> LeaveParty(string partyCode)
        {
            if (partyCode == null)
            {
                RedirectToAction("Index", "Dashboard");
            }

            PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

            if (!await _partyService.LeavePartyAsync(user))
            {
                await _logService.LogUserActivityAsync(user, $"User failed to leave party {partyCode}");
                return RedirectToAction("Index", "Dashboard");
            }

            await _logService.LogUserActivityAsync(user, $"User successfully left party {partyCode}");

            return RedirectToAction("Index", "Dashboard"); ;
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> EndParty()
        {
            PartyGoer host = await _partyGoerService.GetCurrentPartyGoerAsync();

            if (await _partyService.IsUserHostingAPartyAsync(host))
            {
                if (await _partyService.EndPartyAsync(host))
                {
                    await _logService.LogUserActivityAsync(host, $"User successfully ended party");
                    return Ok();
                }
                else
                {
                    await _logService.LogUserActivityAsync(host, $"User failed to end party");
                    return BadRequest("There was an issue with deleting your party");
                }
            }
            else
            {
                await _logService.LogUserActivityAsync(host, $"User failed to end party because they weren't the host");
                return BadRequest("Unable to delete party. You are not hosting any parties");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSongForParty([FromBody]PartyCodeDTO partyCode)
        {
            PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

            if (await _partyService.IsUserPartyingAsync(user))
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                await _logService.LogUserActivityAsync(user, $"User updated song for party with code {partyCode.PartyCode}");

                return await UpdateCurrentSongForEveryoneInPartyAsync(party, user);
            }
            else
            {
                await _logService.LogUserActivityAsync(user, $"User failed tp update song for party with code {partyCode.PartyCode}");
                return BadRequest($"You are currently not hosting a party or attending a party: {partyCode.PartyCode}");
            }
        }

        [Authorize]
        public async Task<IActionResult> UpdateSongForUser()
        {
            try
            {
                PartyGoer listener = await _partyGoerService.GetCurrentPartyGoerAsync();

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
        public async Task<IActionResult> UpdateQueueForParty(string partyCode)
        {
            PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

            Party party = await _partyService.GetPartyWithCodeAsync(partyCode);

            if (party.IsHost(user))
            {

                await UpdatePlaylistForEveryoneInPartyAsync(party, user);

                return Ok();
            }
            else
            {
                return new StatusCodeResult(401);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult App()
        {
            return View();
        }

        private async Task UpdatePlaylistForEveryoneInPartyAsync(Party party, PartyGoer partyGoer)
        {
            var seeds = party.GetSeedUris(5);
            await DomainEvents.RaiseAsync(new QueueEnded { PartyCode = party.GetPartyCode(), SeedTracksUris = seeds.Item1, SeedArtistUris = seeds.Item2 });
        }

        private async Task<IActionResult> UpdateCurrentSongForEveryoneInPartyAsync(Party party, PartyGoer partyGoer)
        {
            try
            {
                await _partyService.UpdateCurrentSongForEveryoneInPartyAsync(party, partyGoer);

                return Ok();
            }
            catch
            {
                return base.StatusCode(500);
            }
        }
    }
}