using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Models.Account;

namespace SpotSync.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Domain.Contracts.IAuthenticationService _authenticationService;
        private readonly IPartyService _partyService;
        private readonly ILogService _logService;

        public AccountController(Domain.Contracts.IAuthenticationService authenticationService, IConfiguration configuration, IPartyService partyService,
        ILogService logService)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
            _partyService = partyService;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginWithSpotifyModel model = new LoginWithSpotifyModel
            {
                ClientId = _configuration.GetValue<string>("Spotify:ClientId"),
                RedirectUrl = _configuration.GetValue<string>("Spotify:RedirectUrl"),
                SpotifyAuthenticationUrl = _configuration.GetValue<string>("Spotify:AuthenticationUrl"),
                SpaceDelimitedScopes = _configuration.GetValue<string>("Spotify:Scopes")
            };

            return Redirect(CreateSpotifyAuthenicateUrl(model));
        }

        public async Task<IActionResult> Authorized(string code)
        {
            try
            {
                string userId = await _authenticationService.AuthenticateUserWithAccessCode(code);

                // Get details from spotify of user 
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                await _logService.LogUserActivityAsync(new PartyGoer(userId), "Successfully authenticated through Spotify");

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                //  TODO: CHANGE THIS TO THE IDNEX PAGE ON HOME
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // If the user is joined in a party, REMOVE HIM
                if (await _partyService.IsUserPartyingAsync(user))
                {
                    Task logUserLeavingParty = _logService.LogUserActivityAsync(user, "Leaving party while logging out");
                    await _partyService.LeavePartyAsync(user);
                    await logUserLeavingParty;
                }

                // If the user is hosting a party, END IT
                if (await _partyService.IsUserHostingAPartyAsync(user))
                {
                    Task logUserEndingParty = _logService.LogUserActivityAsync(user, "Ending party while logging out");
                    await _partyService.EndPartyAsync(user);
                    await logUserEndingParty;
                }

                await _authenticationService.LogOutUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await HttpContext.SignOutAsync();

                await _logService.LogUserActivityAsync(user, "Successfully logged out");
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in Logout()");

            }

            return RedirectToAction("Index", "Home");
        }

        private string CreateSpotifyAuthenicateUrl(LoginWithSpotifyModel model)
        {
            return $"{model.SpotifyAuthenticationUrl}?response_type=code&client_id={model.ClientId}&scope={HttpUtility.UrlEncode(model.SpaceDelimitedScopes)}&redirect_uri={model.RedirectUrl}";
        }
    }
}