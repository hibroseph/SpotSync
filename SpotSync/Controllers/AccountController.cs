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
using SpotSync.Domain.Contracts;
using SpotSync.Models.Account;

namespace SpotSync.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Domain.Contracts.IAuthenticationService _authenticationService;

        public AccountController(Domain.Contracts.IAuthenticationService authenticationService, IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
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

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogOutUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        private string CreateSpotifyAuthenicateUrl(LoginWithSpotifyModel model)
        {
            return $"{model.SpotifyAuthenticationUrl}?response_type=code&client_id={model.ClientId}&scope={HttpUtility.UrlEncode(model.SpaceDelimitedScopes)}&redirect_uri={model.RedirectUrl}";
        }
    }
}