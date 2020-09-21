using Microsoft.AspNetCore.SignalR;
using SpotSync.Classes.Hubs;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes
{
    public class PartyHandler : IHandles<ChangeSong>
    {
        private readonly ISpotifyHttpClient _spotifyHttpClient;
        private readonly IHubContext<PartyHub> _partyHubContext;
        private readonly ILogService _logService;

        public PartyHandler(ISpotifyHttpClient spotifyHttpClient, IHubContext<PartyHub> partyHubContext, ILogService logService)
        {
            _logService = logService;
            _partyHubContext = partyHubContext;
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task HandleAsync(ChangeSong args)
        {
            await _logService.LogAppActivityAsync($"Updating song for party with code {args.PartyCode}. New song artist: {args.Song.Artist}, title: {args.Song.Title}");

            await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdateSong", args.Song, args.ProgressMs);

            return;
        }
    }
}
