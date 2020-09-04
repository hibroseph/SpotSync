using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
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
        private IPlaylistService _playlistService;
        public PartyHub(IPartyService partyService, IPlaylistService playlistService)
        {
            _partyService = partyService;
            _playlistService = playlistService;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.UserIdentifier, message);
        }

        public async Task ConnectToParty(string partyCode)
        {
            if (await _partyService.IsUserPartyingAsync(new PartyGoer(Context.UserIdentifier)) || _partyService.IsUserHostingAParty(new PartyGoer(Context.UserIdentifier)))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, partyCode);
                await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} has joined the party {partyCode}");

                return;
            }
        }

        public async Task UpdateParty(string partyCode)
        {
            await Clients.Group(partyCode).SendAsync("UpdateParty", $"{Context.UserIdentifier} updated the party at {DateTime.UtcNow}");
        }

        public async Task CreatePlaylist(string partyCode)
        {

        }
    }
}
