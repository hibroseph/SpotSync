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
using SpotSync.Models.Party;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Classes.Hubs
{
    [Authorize]
    public class PartyHub : Hub
    {
        private IPartyService _partyService;
        private IPartyGoerService _partyGoerService;
        private ISpotifyHttpClient _spotifyHttpClient;
        private ILogService _logService;

        private const string UPDATE_TRACK_FEELINGS_ENDPOINT = "UpdateTrackVotes";

        public PartyHub(IPartyService partyService, ISpotifyHttpClient spotifyHttpClient, ILogService logService, IPartyGoerService partyGoerService)
        {
            _partyService = partyService;
            _spotifyHttpClient = spotifyHttpClient;
            _logService = logService;
            _partyGoerService = partyGoerService;
        }

        public async Task AddTrackFeeling(string partyCode, string trackUri, int feeling)
        {
            Party party = await _partyService.GetPartyWithCodeAsync(partyCode);
            PartyGoer user = await _partyGoerService.GetCurrentPartyGoerAsync();

            switch (feeling)
            {
                case 0:
                    await party.UserDislikesTrackAsync(user, trackUri);
                    break;
                case 1:
                    await party.UserLikesTrackAsync(user, trackUri);
                    break;
            }
            await Clients.Group(partyCode).SendAsync(UPDATE_TRACK_FEELINGS_ENDPOINT, party.GetTrackVotes());


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
            await Clients.GroupExcept(partyCode, new List<string> { Context.ConnectionId }).SendAsync("NewListener", Context.UserIdentifier);

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
                Listeners = ConvertToListenerModel(party.GetListeners()),
                Host = party.GetHost().GetId()
            }
            ); ;

            // check for explicit music
            if (!partier.CanListenToExplicitSongs() && party.HasExplicitTracks())
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ExplicitSong", "You have filtering explicit music turned on in Spotify and there are explicit songs in the queue. We will not play the explicit song for you but continue playback when a non explicit song comes on.");
            }

            await Clients.Client(Context.ConnectionId).SendAsync("InitializeWebPlayer", await _partyGoerService.GetPartyGoerAccessTokenAsync(partier));

            // make sure that the users spotify is connected
            if (string.IsNullOrEmpty(await _spotifyHttpClient.GetUsersActiveDeviceAsync(partier.GetId())))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ConnectSpotify", "");
            }

            await _logService.LogUserActivityAsync(partier, $"Joined real time collobration in party with code {partyCode}");
            return;

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();
            Party party = await _partyService.GetPartyWithAttendeeAsync(partier);

            if (party != null)
            {
                await Clients.Group(party.GetPartyCode()).SendAsync("ListenerLeft", Context.UserIdentifier);
                party.LeaveParty(partier);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private List<string> ConvertToListenerModel(List<PartyGoer> partyGoers)
        {
            List<string> listeners = new List<string>();

            foreach (PartyGoer partyGoer in partyGoers)
            {
                listeners.Add(partyGoer.GetId());
            }

            return listeners;
        }

        public async Task WebPlayerInitialized(string device_id)
        {
            PartyGoer partyGoer = await _partyGoerService.GetCurrentPartyGoerAsync();

            partyGoer.AddPerferredDeviceId(device_id);

            await _partyService.SyncListenerWithSongAsync(partyGoer);

        }

        public async Task UserWantsToSkipSong(string partyCode)
        {
            //  TODO: Add validation, user is in party
            await _partyService.UserWantsToSkipSong(await _partyGoerService.GetCurrentPartyGoerAsync(), partyCode);
        }

        public async Task UserAddedSong(AddSongToQueueCommand request)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();

            request.AddedBy = partier.GetUsername();
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

        public async Task AddSomeTracksFromPlaylistToQueue(string partyCode, string playlistId, int amount)
        {
            PartyGoer partier = await _partyGoerService.GetCurrentPartyGoerAsync();

            await _partyService.AddSomeTracksFromPlaylistToQueueAsync(partier, playlistId, amount);

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

        public async Task NukeQueue(string partyCode)
        {
            Party party = await _partyService.GetPartyWithCodeAsync(partyCode);

            party.NukeQueue(await _partyGoerService.GetCurrentPartyGoerAsync());

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

        public async Task UpdateSongForParty(string partyCode, Track currentSong, int currentProgressInMs)
        {
            await Clients.Group(partyCode).SendAsync("UpdateSong", currentSong);
        }
    }
}
