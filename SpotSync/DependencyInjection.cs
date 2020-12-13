using Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using SpotSync.Application.Authentication;
using SpotSync.Application.Services;
using SpotSync.Classes;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Repositories;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;
using SpotSync.Infrastructure;
using System;

namespace SpotSync
{
    public static class DependencyInjection
    {
        public static void AddSpotSyncServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<ISpotifyAuthentication>(new SpotifyAuthentication(configuration["Spotify:ClientId"], configuration["Spotify:ClientSecret"], configuration["Spotify:RedirectUrl"]));
            serviceCollection.AddSingleton<IHttpClient>(new HttpClient());

            serviceCollection.AddSingleton<ISpotifyHttpClient, SpotifyHttpClient>();
            serviceCollection.AddSingleton<ILogRepository>(new LogRepository(configuration["DatabaseConnection"]));

            serviceCollection.AddSingleton<IPartyRepository, PartyRepository>();
            serviceCollection.AddSingleton<IPartyGoerService, PartyGoerService>();
            serviceCollection.AddSingleton<ILogService, LogService>();
            serviceCollection.AddSingleton<IPartyService, PartyService>();
            serviceCollection.AddSingleton<IAuthenticationService, AuthenticationService>();

            serviceCollection.AddSingleton<IHandles<ChangeSong>, PartyHandler>();
            serviceCollection.AddSingleton<IHandles<PlaylistEnded>, PartyHandler>();
            serviceCollection.AddSingleton<IHandles<ToggleMusicState>, PartyHandler>();

            serviceCollection.AddHttpContextAccessor();
        }

        public static void AddSpotSyncAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        }

        public static void AddAndStartDatabaseMigration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            DatabaseMigration migration = new DatabaseMigration(new DatabaseMigrationConfig(configuration["DatabaseConnection"]));

            migration.Update();
        }
    }
}