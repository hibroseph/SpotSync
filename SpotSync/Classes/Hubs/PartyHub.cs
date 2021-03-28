using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Contracts.Services.PartyGoerSetting;
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
        private IPartyGoerService _partyGoerService;
        private ISpotifyHttpClient _spotifyHttpClient;
        private ILogService _logService;
        private IPartyGoerSettingsService _partyGoerSettingsService;

        public PartyHub(IPartyService partyService, ISpotifyHttpClient spotifyHttpClient, ILogService logService, IPartyGoerService partyGoerService, IPartyGoerSettingsService partyGoerSettingsService)
        {
            _partyService = partyService;
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
            _partyGoerService = partyGoerService;
            _partyGoerSettingsService = partyGoerSettingsService;
        }

        public async Task ConnectToParty(string partyCode)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();
            if (!await _partyService.IsUserPartyingAsync(partier) && !await _partyService.IsUserHostingAPartyAsync(partier))
            {
                await _partyService.JoinPartyAsync(new Domain.DTO.PartyCodeDTO { PartyCode = partyCode }, partier);
                await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
                await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");
            }

            // Add the partier to real-time connection group
            await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
            await Clients.Group(partyCode).SendAsync("NewListener", Context.UserIdentifier);

            Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

            // Update the view of the partier to the current playlist
            await Clients.Client(Context.ConnectionId).SendAsync("InitialPartyLoad",
            new
            {
                Song = party.GetCurrentSong(),
                Position = party.GetCurrentPositionInSong()
            },
            party.GetHistory(),
            party.GetQueue(),
            new
            {
                PartyCode = party.GetPartyCode(),
                Listeners = party.GetListeners()
            }
            );

            // check for explicit music
            if (partier.FilterExplicitSongs && party.HasExplicitTracks())
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ExplicitSong", "You have filtering explicit music turned on in Spotify and there are explicit songs in the queue. We will not play the explicit song for you but continue playback when a non explicit song comes on.");
            }

            await Clients.Client(Context.ConnectionId).SendAsync("InitializeWebPlayer", await _partyGoerService.GetPartyGoerAccessTokenAsync(partier));

            // make sure that the users spotify is connected
            if (string.IsNullOrEmpty(await _spotifyHttpClient.GetUsersActiveDeviceAsync(partier.Id)))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ConnectSpotify", "");
            }

            await _logService.LogUserActivityAsync(partier, $"Joined real time collobration in party with code {partyCode}");
            return;

        }

        public async Task WebPlayerInitialized(string device_id)
        {
            PartyGoer partyGoer = await _partyGoerService.GetCurrentPartyGoerAsync();

            _partyGoerSettingsService.SetConfigurationSetting(partyGoer, new PartyGoerConfigurationSetting { PerferredDeviceId = device_id });

            await _partyService.SyncListenerWithSongAsync(partyGoer);

        }

        public async Task UserWantsToSkipSong(string partyCode)
        {
            //  TODO: Add validation, user is in party
            await _partyService.UserWantsToSkipSong(await _partyGoerService.GetCurrentPartyGoerAsync(), partyCode);
        }

        public async Task UserAddedSong(AddSongToQueueRequest request)
        {
            PartyGoer partier = new PartyGoer(Context.UserIdentifier);
            request.AddedBy = partier.Id;
            bool successfullyAddedSongToQueue = await _partyService.AddNewSongToQueue(request);

            if (successfullyAddedSongToQueue)
            {
                Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

                // Update the view of the partier to the current playlist
                await Clients.Group(party.GetPartyCode()).SendAsync("UpdatePartyView",
                new
                {
                    Song = party.GetCurrentSong(),
                    Position = party.GetCurrentPositionInSong()
                },
                party.GetHistory(),
                party.GetQueue()
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
                await Clients.Group(party.GetPartyCode()).SendAsync("UpdatePartyView",
                new
                {
                    Song = party.GetCurrentSong(),
                    Position = party.GetCurrentPositionInSong()
                },
                party.GetHistory(),
                party.GetQueue()
                );
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UserModifiedPlaylist", new { error = true });
            }
        }

        public async Task UpdateSongForParty(string partyCode, Track currentSong, int currentProgressInMs)
        {
            await Clients.Group(partyCode).SendAsync("UpdateSong", currentSong);
        }

        public async Task UpdatePlaylistForParty(string partyCode, List<Track> playlist)
        {
            await Clients.Group(partyCode).SendAsync("UpdatePlaylist", playlist, playlist.First());
        }
    }
}
