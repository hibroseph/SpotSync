using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
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
            ISpotifyHttpClient spotifyHttpClient = new SpotifyHttpClient(configuration["Spotify:ClientId"], configuration["Spotify:ClientSecret"], configuration["Spotify:RedirectUrl"]);
            IPartyRepository partyRepository = new PartyRepository();
            IPartyGoerService partyGoerService = new PartyGoerService(spotifyHttpClient);

            serviceCollection.AddSingleton<IPartyService>(new PartyService(partyRepository, partyGoerService, spotifyHttpClient));
            serviceCollection.AddSingleton<IAuthenticationService>(new AuthenticationService(spotifyHttpClient));
            serviceCollection.AddSingleton<IPartyGoerService>(partyGoerService);
        }

        public static void AddSpotSyncAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        }
    }
}
