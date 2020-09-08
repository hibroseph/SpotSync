using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;
using SpotSync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes.Hubs
{
    [Authorize]
    public class PartyHub : Hub
    {
        private IPartyService _partyService;
        private ISpotifyHttpClient _spotifyHttpClient;
        public PartyHub(IPartyService partyService, ISpotifyHttpClient spotifyHttpClient)
        {
            _partyService = partyService;
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.UserIdentifier, message);

        }

        public async Task ConnectToParty(string partyCode)
        {
            var userId = new PartyGoer(Context.UserIdentifier);
            if (await _partyService.IsUserPartyingAsync(userId) || _partyService.IsUserHostingAParty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
                await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");

                // TODO: Add code and logic to determine if a playlist is currently playing and to sync the current joining users spotify to where everyone is in the song

                Party party = await _partyService.GetPartyWithAttendeeAsync(userId);

                if (party.IsPartyPlayingMusic())
                {
                    // update spotify to play current position 
                    await _spotifyHttpClient.UpdateSongForPartyGoerAsync(userId.Id, new List<string> { party.GetSongPlaying().TrackUri }, party.GetSongPosition());
                    await Clients.Client(Context.ConnectionId).SendAsync("UpdatePlaylist", party.Playlist.Queue);
                    await Clients.Client(Context.ConnectionId).SendAsync("UpdateSong", party.GetSongPlaying());
                }

                return;
            }
        }

        public async Task UpdateParty(string partyCode)
        {
            await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} updated the party at {DateTime.UtcNow}");
        }

        public async Task UpdateSongForParty(string partyCode, Song currentSong, int currentProgressInMs)
        {
            await Clients.Group(partyCode).SendAsync("UpdateSong", currentSong);
        }

        public async Task UpdatePlaylistForParty(string partyCode, List<Song> playlist)
        {
            await Clients.Group(partyCode).SendAsync("UpdatePlaylist", playlist);
        }

    }
}
