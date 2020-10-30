using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;
using SpotSync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace SpotSync.Classes.Hubs
{
    [Authorize]
    public class PartyHub : Hub
    {
        private IPartyService _partyService;
        private ISpotifyHttpClient _spotifyHttpClient;
        private ILogService _logService;

        public PartyHub(IPartyService partyService, ISpotifyHttpClient spotifyHttpClient, ILogService logService)
        {
            _partyService = partyService;
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
        }

        public async Task ConnectToParty(string partyCode)
        {
            var partier = new PartyGoer(Context.UserIdentifier);
            if (!await _partyService.IsUserPartyingAsync(partier) && !await _partyService.IsUserHostingAPartyAsync(partier))
            {
                await _partyService.JoinPartyAsync(new Domain.DTO.PartyCodeDTO { PartyCode = partyCode }, partier);
                await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
                await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");
            }

            // Add the partier to real-time connection group
            await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
            await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");
            await Clients.Group(partyCode).SendAsync("NewListener", Context.UserIdentifier);

            Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

            // Update the view of the partier to the current playlist
            await Clients.Client(Context.ConnectionId).SendAsync("UpdatePartyView",
            new
            {
                Song = party.Playlist.CurrentSong,
                Position = party.Playlist.CurrentPositionInSong()
            },
            party.Playlist.History,
            party.Playlist.Queue
            );

            // make sure that the users spotify is connected
            if (string.IsNullOrEmpty(await _spotifyHttpClient.GetUsersActiveDeviceAsync(partier.Id)))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ConnectSpotify", "");
            }
            await _logService.LogUserActivityAsync(partier, $"Joined real time collobration in party with code {partyCode}");
            return;

        }

        public async Task UserAddedSong(AddSongToQueueRequest request)
        {
            PartyGoer partier = new PartyGoer(Context.UserIdentifier);
            bool successfullyAddedSongToQueue = await _partyService.AddNewSongToQueue(request);

            if (successfullyAddedSongToQueue)
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

                // Update the view of the partier to the current playlist
                await Clients.Group(party.PartyCode).SendAsync("UpdatePartyView",
                new
                {
                    Song = party.Playlist.CurrentSong,
                    Position = party.Playlist.CurrentPositionInSong()
                },
                party.Playlist.History,
                party.Playlist.Queue
                );
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UserModifiedPlaylist", new { error = true });
            }
        }

        public async Task UserModifiedPlaylist(RearrangeQueueRequest queueRequest)
        {
            var partier = new PartyGoer(Context.UserIdentifier);
            bool successfullyRearrangedQueue = await _partyService.RearrangeQueue(queueRequest);

            if (successfullyRearrangedQueue)
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

                // Update the view of the partier to the current playlist
                await Clients.Group(party.PartyCode).SendAsync("UpdatePartyView",
                new
                {
                    Song = party.Playlist.CurrentSong,
                    Position = party.Playlist.CurrentPositionInSong()
                },
                party.Playlist.History,
                party.Playlist.Queue
                );
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UserModifiedPlaylist", new { error = true });
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
            await Clients.Group(partyCode).SendAsync("UpdatePlaylist", playlist, playlist.First());
        }
    }
}
