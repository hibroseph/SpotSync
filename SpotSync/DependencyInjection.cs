using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using SpotSync.Application.Authentication;
using SpotSync.Application.Services;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync
{
    public static class DependencyInjection
    {
        public static void AddSpotSyncServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            SpotifyAuthentication spotifyAuthentication = new SpotifyAuthentication(configuration["Spotify:ClientId"], configuration["Spotify:ClientSecret"], configuration["Spotify:RedirectUrl"]);

            ISpotifyHttpClient spotifyHttpClient = new SpotifyHttpClient(spotifyAuthentication, new HttpClient());
            IPartyRepository partyRepository = new PartyRepository();
            IPartyGoerService partyGoerService = new PartyGoerService(spotifyHttpClient);

            serviceCollection.AddSingleton<IPartyService>(new PartyService(partyRepository, spotifyHttpClient));
            serviceCollection.AddSingleton<IAuthenticationService>(new AuthenticationService(spotifyHttpClient, spotifyAuthentication));
            serviceCollection.AddSingleton<IPartyGoerService>(partyGoerService);
        }

        public static void AddSpotSyncAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        }
    }
}
