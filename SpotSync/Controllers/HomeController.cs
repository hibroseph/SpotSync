﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Models;
using SpotSync.Models.Shared;

namespace SpotSync.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPartyService _partyService;
        public HomeController(ILogger<HomeController> logger, IPartyService partyService)
        {
            _logger = logger;
            _partyService = partyService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //return RedirectToAction("Index", "Dashboard");
                PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

                Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                return View(new BaseModel(party != null ? true : false, party?.GetPartyCode()));
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
