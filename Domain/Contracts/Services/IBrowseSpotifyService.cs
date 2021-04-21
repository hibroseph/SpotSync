using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IBrowseSpotifyService
    {
        Task<SpotibroModels.ArtistInformation> GetArtistInformationAsync(PartyGoer partyGoer, string artistId);
    }
}
