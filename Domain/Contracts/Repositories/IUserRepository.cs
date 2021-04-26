using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task FavoriteTrackAsync(int userId, string trackUri);
        Task UnfavoriteTrackAsync(int userId, string trackUri);
        Task<List<string>> GetFavoriteTracksAsync(int partyGoerId);
        Task<int?> SaveUserAsync(PartyGoer partier);
        Task<int?> GetUserIdAsync(PartyGoer user);
        Task UpdateLastLoginTimeAsync(int userId);
    }
}
