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
    public class PartyHandler : IHandles<ChangeSong>, IHandles<PlaylistEnded>
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

        public async Task HandleAsync(ChangeSong args)
        {
            await _logService.LogAppActivityAsync($"Updating song for party with code {args.PartyCode}. New song artist: {args.Song.Artist}, title: {args.Song.Name}");

            // TODO: Make this a parallel
            foreach (PartyGoer listener in args.Listeners)
            {
                try
                {
                    await _logService.LogAppActivityAsync($"Updating song for PartyGoer {listener.Id}. New song artist: {args.Song.Artist}, title: {args.Song.Name}");
                    await _spotifyHttpClient.UpdateSongForPartyGoerAsync(listener.Id, args.Song.Uri, args.ProgressMs);
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

            await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdateSong", new { song = args.Song, position = args.ProgressMs });

            return;
        }

        public async Task HandleAsync(PlaylistEnded args)
        {
            await _logService.LogAppActivityAsync($"Playlist has ended for party with code {args.PartyCode}. Sending to all listeners");

            Party party = await _partyService.GetPartyWithCodeAsync(args.PartyCode);

            if (party.Listeners.Count < 1)
            {
                await party.DeletePlaylistAsync();
                await _partyService.EndPartyAsync(args.PartyCode);
                return;
            }

            List<Track> playlistSongs = await GenerateNewPlaylist(party);
            // If there is atleast 1 person still in the party, regenerate the playlist
            party.Playlist = new Playlist(playlistSongs, party.Listeners, party.PartyCode, party.Playlist.History);

            await party.Playlist.StartAsync();

            await _partyHubContext.Clients.Group(args.PartyCode).SendAsync("UpdatePartyView",
            new
            {
                Song = party.Playlist.CurrentSong,
                Position = party.Playlist.CurrentPositionInSong()
            },
            party.Playlist.History,
            party.Playlist.Queue
            );
        }

        private async Task<List<Track>> GenerateNewPlaylist(Party party)
        {
            if (party.Playlist.History.Count > 0)
            {
                return await _spotifyHttpClient.GetRecommendedSongsAsync(party.Listeners.ElementAt(0).Id, party.Playlist.History.ToList().GetRandomNItems(5).Select(p => p.Uri).ToList(), 0);
            }
            else
            {
                List<Track> songs = new List<Track>();

                foreach (PartyGoer listener in party.Listeners)
                {
                    songs.AddRange(await _partyGoerService.GetRecommendedSongsAsync(listener.Id, 2));
                }

                return await _spotifyHttpClient.GetRecommendedSongsAsync(party.Listeners.ElementAt(0).Id, songs.GetRandomNItems(5).Select(p => p.Uri).ToList(), 0);
            }
        }
    }
}
