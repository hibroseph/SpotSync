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
            var partier = new PartyGoer(Context.UserIdentifier);
            if (await _partyService.IsUserPartyingAsync(partier) || _partyService.IsUserHostingAParty(partier))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
                await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");
            }

            Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

            if (party != null && party.IsPartyPlayingMusic())
            {
                // update spotify to play current position 
                await _spotifyHttpClient.UpdateSongForPartyGoerAsync(partier.Id, new List<string> { party.GetSongPlaying().TrackUri }, party.GetSongPosition());
                await Clients.Client(Context.ConnectionId).SendAsync("UpdatePlaylist", party.Playlist.Queue, party.GetSongPosition());
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateSong", party.GetSongPlaying(), party.GetSongPosition());
            }

            party = await _partyService.GetPartyWithHostAsync(partier);

            if (party != null && party.IsPartyPlayingMusic())
            {
                // update spotify to play current position 
                await _spotifyHttpClient.UpdateSongForPartyGoerAsync(partier.Id, new List<string> { party.GetSongPlaying().TrackUri }, party.GetSongPosition());
                await Clients.Client(Context.ConnectionId).SendAsync("UpdatePlaylist", party.Playlist.Queue, party.GetSongPosition());
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateSong", party.GetSongPlaying(), party.GetSongPosition());
            }

            return;
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
            await Clients.Group(partyCode).SendAsync("UpdatePlaylist", playlist, playlist.First());
        }

    }
}
