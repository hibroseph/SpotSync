using Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using SpotSync.Application.Authentication;
using SpotSync.Application.Services;
using SpotSync.Classes;
using SpotSync.Classes.Authorization;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Repositories;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;
using SpotSync.Infrastructure.SpotifyApi;
using SpotSync.Infrastructure;
using System;
using SpotSync.Domain.Contracts.SpotifyApi;
using System.Collections.Generic;
using SpotSync.Domain.Contracts.SpotifyApi.Models;

namespace SpotSync
{
    public static class DependencyInjection
    {
        public static void AddSpotSyncServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IAuthorizationHandler, DiagnosticsKeyRequirementHandler>();
            serviceCollection.AddSingleton<ISpotifyAuthentication>(new SpotifyAuthentication(configuration["Spotify:ClientId"], configuration["Spotify:ClientSecret"], configuration["Spotify:RedirectUrl"]));
            serviceCollection.AddSingleton<IHttpClient>(new HttpClient());


            serviceCollection.AddSingleton<ISpotifyHttpClient, SpotifyHttpClient>();
            serviceCollection.AddSingleton<ILogRepository>(new LogRepository(configuration["DatabaseConnection"]));
            serviceCollection.AddSingleton<IUserRepository>(new UserRepository(configuration["DatabaseConnection"]));
            serviceCollection.AddSingleton<ISpotifyApi, SpotifyApi>();
            serviceCollection.AddSingleton<IPartyRepository, PartyRepository>();
            serviceCollection.AddSingleton<IPartyGoerService, PartyGoerService>();
            serviceCollection.AddSingleton<IBrowseSpotifyService, BrowseSpotifyService>();
            serviceCollection.AddSingleton<ILogService, LogService>();
            serviceCollection.AddSingleton<IPartyService, PartyService>();
            serviceCollection.AddSingleton<IAuthenticationService, AuthenticationService>();

            serviceCollection.AddSingleton<IDiagnosticsService, DiagnosticsService>();

            serviceCollection.AddSingleton<IHandles<ChangeTrack>, PartyHandler>();
            serviceCollection.AddSingleton<IHandles<QueueEnded>, PartyHandler>();
            serviceCollection.AddSingleton<IHandles<ToggleMusicState>, PartyHandler>();
            serviceCollection.AddSingleton<IHandles<UpdateQueue>, PartyHandler>();

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