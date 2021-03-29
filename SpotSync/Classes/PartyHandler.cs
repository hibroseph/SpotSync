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

namespace SpotSync.Classes
{
    public class PartyHandler : IHandles<ChangeTrack>, IHandles<PlaylistEnded>, IHandles<ToggleMusicState>
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
            await _logService.LogAppActivityAsync($"Updating song for party with code {args.PartyCode}. New song artist: {args.Track.Artist}, title: {args.Track.Name}");

            // TODO: Make this a parallel
            foreach (PartyGoer listener in args.Listeners)
            {
                if (!listener.PausedMusic)
                {
                    try
                    {
                        await _logService.LogAppActivityAsync($"Updating song for PartyGoer {listener.Id}. New song artist: {args.Track.Artist}, title: {args.Track.Name}");
                        await _spotifyHttpClient.UpdateSongForPartyGoerAsync(listener, new List<string> { args.Track.Uri }, args.ProgressMs);
                    }
                    catch (NoActiveDeviceException)
                    {
                        await _partyHubContext.Clients.User(listener.Id).SendAsync("ConnectSpotify", "GOOD MSG");
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

        public async Task HandleAsync(PlaylistEnded args)
        {
            await _logService.LogAppActivityAsync($"Playlist has ended for party with code {args.PartyCode}. Sending to all listeners");

            Party party = await _partyService.GetPartyWithCodeAsync(args.PartyCode);

            if (party.GetListeners().Count < 1)
            {
                await _partyService.EndPartyAsync(args.PartyCode);
                return;
            }

            List<Track> playlistSongs = await GenerateNewPlaylist(party, args.LikedTracksUris);

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

        private async Task<List<Track>> GenerateNewPlaylist(Party party, List<string> recommendedTrackUris)
        {
            if (recommendedTrackUris.Count > 0)
            {
                return await _spotifyHttpClient.GetRecommendedSongsAsync(party.GetHost().Id, recommendedTrackUris, 0);
            }
            else
            {
                return await _spotifyHttpClient.GetRecommendedSongsAsync(party.GetHost().Id, (await _partyGoerService.GetRecommendedSongsAsync(party.GetHost().Id, 2)).Select(track => track.Uri).ToList(), 0);
            }

        }
    }
}
