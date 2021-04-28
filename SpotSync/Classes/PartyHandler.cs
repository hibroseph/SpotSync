using Microsoft.AspNetCore.SignalR;
using SpotSync.Classes.Hubs;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Errors;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Classes
{
    public class PartyHandler : IHandles<ChangeTrack>, IHandles<QueueEnded>, IHandles<ToggleMusicState>, IHandles<UpdateQueue>
    {
        private readonly ISpotifyHttpClient _spotifyHttpClient;
        private readonly IHubContext<PartyHub> _partyHubContext;
        private readonly IPartyService _partyService;
        private readonly IPartyGoerService _partyGoerService;
        private readonly ILogService _logService;

        public PartyHandler(ISpotifyHttpClient spotifyHttpClient, IHubContext<PartyHub> partyHubContext, ILogService logService, IPartyService partyService, IPartyGoerService partyGoerService)
        {
            _logService = logService;
            _partyHubContext = partyHubContext;
            _spotifyHttpClient = spotifyHttpClient;
            _partyService = partyService;
            _partyGoerService = partyGoerService;
        }

        public async Task HandleAsync(ChangeTrack args)
        {
            if (args.Track != null)
            {
                // TODO: Make this a parallel
                foreach (PartyGoer listener in args.Listeners)
                {
                    if (!listener.IsMusicPaused())
                    {
                        try
                        {
                            await _spotifyHttpClient.UpdateSongForPartyGoerAsync(listener, new List<string> { args.Track.Id }, args.ProgressMs);
                        }
                        catch (NoActiveDeviceException)
                        {
                            await _partyHubContext.Clients.User(listener.GetId()).SendAsync("ConnectSpotify", "GOOD MSG");
                        }
                        catch (Exception ex)
                        {
                            await _logService.LogExceptionAsync(ex, "Error occurred in HandleAsync()");
                        }
                    }
                }

                await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdateSong", new { song = args.Track, position = args.ProgressMs });

                return;
            }
        }

        public async Task HandleAsync(ToggleMusicState args)
        {
            if (args.State == Domain.Types.PlaybackState.Pause)
            {
                await _spotifyHttpClient.TogglePlaybackAsync(args.Listener, args.State);
            }
            else
            {
                await _partyService.SyncListenerWithSongAsync(args.Listener);
            }
        }

        public async Task HandleAsync(QueueEnded args)
        {
            await _logService.LogAppActivityAsync($"Playlist has ended for party with code {args.PartyCode}. Sending to all listeners");

            Party party = await _partyService.GetPartyWithCodeAsync(args.PartyCode);

            if (party.GetListeners().Count < 1)
            {
                await _partyService.EndPartyAsync(args.PartyCode);
                return;
            }

            List<Track> playlistSongs = await _partyService.GenerateNewPlaylist(party, args.SeedTracksUris, args.SeedArtistUris);

            await party.AddNewQueueAsync(playlistSongs);

            await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdatePartyView",
            new
            {
                Song = party.GetCurrentSong(),
                Position = party.GetCurrentPositionInSong()
            },
            party.GetHistory(),
            party.GetQueue()
            );
        }

        public async Task HandleAsync(UpdateQueue args)
        {
            await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdateQueue", args.Tracks);
        }
    }
}
