using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IPlaylistService
    {
        void CreatePlaylist(PartyCodeDTO partyCode);
        void StartPlaylist(PartyCodeDTO partyCode);
        void StopPlaylist(PartyCodeDTO partyCode);
    }
}
