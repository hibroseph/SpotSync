using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IBrowseSpotifyService
    {
        Task<ArtistInformation> GetArtistInformationAsync(PartyGoer partyGoer, string artistId);
    }
}
