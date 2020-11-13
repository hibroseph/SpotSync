using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Models.Error;
using SpotSync.Models.Shared;

namespace SpotSync.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IPartyGoerService _partyGoerService;
        private readonly ILogService _logService;
        public ErrorController(IPartyGoerService partyGoerService, ILogService logService)
        {
            _partyGoerService = partyGoerService;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string exceptionId)
        {
            if (!string.IsNullOrWhiteSpace(exceptionId))
            {
                // The exception has already been logged 
                return View(new BaseModel<ErrorModel>(new ErrorModel { ExceptionId = exceptionId }, new BaseModel()));
            }

            // We need to try to find the exception that occurred
            IExceptionHandlerFeature exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();

            PartyGoer currentUser = null;
            bool userIsLoggedIn = false;

            // Try to get the current user if it exists
            if (!string.IsNullOrWhiteSpace(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                currentUser = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
                userIsLoggedIn = true;
            }

            string referenceId = null;

            if (exceptionHandler.Error != null)
            {
                referenceId = await _logService.LogExceptionAsync(exceptionHandler.Error, $"Error occurred in app{(userIsLoggedIn ? $" with user {currentUser.Id}" : string.Empty)}");
            }
            else
            {
                referenceId = await _logService.LogExceptionAsync(new Exception($"There was an issue{(userIsLoggedIn ? $" with user { currentUser.Id }" : string.Empty)}"), "There was no exception caught. Creating a new one");
            }

            return View(new BaseModel<ErrorModel>(new ErrorModel { ExceptionId = referenceId }, new BaseModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Index(BaseModel<ErrorModel> baseModel)
        {
            ErrorModel model = baseModel.PageModel;
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Description))
                {
                    ModelState.AddModelError(nameof(model.Description), "The error description cannot be empty");
                }

                return View(baseModel);
            }

            await _logService.AddDescriptionToExceptionAsync(model.Description, model.ExceptionId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View("Reported");
        }
    }
}