using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;

namespace SpotSync.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly IDiagnosticsService _diagnosticsService;
        public DiagnosticsController(IDiagnosticsService diagnosticsService)
        {
            _diagnosticsService = diagnosticsService;
        }

        [Authorize(Policy = "DiagnosticsPolicy")]
        [Route("api/[controller]/parties")]
        public async Task<IActionResult> GetAllParties()
        {
            return new JsonResult(await _diagnosticsService.GetPartyDiagnosticsAsync());
        }
    }
}