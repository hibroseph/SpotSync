using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface ISpotifyHttpClient
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
        Task<string> RequestAccessAndRefreshTokenFromSpotifyAsync(string code);
        Task<bool> UpdateSongForPartyGoerAsync(string partyGoerId, CurrentSongDTO currentSong);
    }
}
