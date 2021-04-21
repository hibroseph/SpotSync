﻿using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IPartyGoerService
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
        Task<Domain.Contracts.SpotifyApi.Models.SearchTracks> GetRecommendedSongsAsync(string partyGoerId, int count = 10);
        Task<string> GetUsersActiveDeviceAsync(string partyGoerId);
        Task<IEnumerable<ISpotifyQueryResult>> SearchSpotifyAsync(string query, SpotifyQueryType queryType, int limit = 10);
        Task<PartyGoer> GetCurrentPartyGoerAsync();
        Task<string> GetPartyGoerAccessTokenAsync(PartyGoer partyGoer);
        Task<ServiceResult<List<Device>>> GetUserDevicesAsync(PartyGoer partyGoer);
        Task<List<SpotibroModels.PlaylistSummary>> GetUsersPlaylistsAsync(PartyGoer user, int limit = 10, int offset = 0);
        Task<SpotibroModels.PlaylistContents> GetPlaylistItemsAsync(PartyGoer user, string playlistId);
    }
}
