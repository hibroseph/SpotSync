using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain.Contracts.SpotifyApi
{
    public interface ISpotifyApi
    {
        Task<List<PlaylistSummary>> GetUsersPlaylistsAsync(PartyGoer partyGoer, int limit, int offset);
        Task<PlaylistContents> GetPlaylistContentsAsync(PartyGoer partyGoer, string playlistId);
        Task<List<Artist>> GetUsersTopArtistsAsync(PartyGoer partyGoer, int amount = 10);
    }
}
