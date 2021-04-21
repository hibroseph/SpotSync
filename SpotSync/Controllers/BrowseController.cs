using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Classes.Responses.Common;
using SpotSync.Domain.Contracts.Services;

namespace SpotSync.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BrowseController : ControllerBase
    {
        private IBrowseSpotifyService _browseSpotifyService;
        private IPartyGoerService _partyGoerService;
        public BrowseController(IBrowseSpotifyService browseSpotifyService, IPartyGoerService partyGoerService)
        {
            _browseSpotifyService = browseSpotifyService;
            _partyGoerService = partyGoerService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchArtist(string artistId)
        {
            try
            {
                return new JsonResult(await _browseSpotifyService.GetArtistInformationAsync(await _partyGoerService.GetCurrentPartyGoerAsync(), artistId));
            }
            catch (Exception)
            {
                // TODO: return a 500
                return new JsonResult(new Result(false, "Unable to get artist information"));
            }
        }
    }
}