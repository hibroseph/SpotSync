using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IPartyGoerService
    {
        Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId);
    }
}
