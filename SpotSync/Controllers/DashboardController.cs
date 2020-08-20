using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Models.Dashboard;


namespace SpotSync.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IPartyGoerService _partyGoerService;

        public DashboardController(IPartyService partyService, IPartyGoerService partyGoerService)
        {
            _partyService = partyService;
            _partyGoerService = partyGoerService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            CurrentSongDTO currentSong = await _partyGoerService.GetCurrentSongAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            DashboardModel model = new DashboardModel
            {
                Name = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CurrentSong = currentSong != null ? new Song
                {
                    Title = currentSong.Title,
                    Artist = currentSong.Artist,
                    Album = currentSong.Album,
                    AlbumImageUrl = currentSong.AlbumArtUrl
                } : null
            };

            return View(model);
        }


    }
}