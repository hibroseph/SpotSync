using Dapper;
using Npgsql;
using SpotSync.Domain;
using SpotSync.Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence
{
    public class UserRepository : IUserRepository
    {
        private NpgsqlConnection _connection;

        public UserRepository(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        public async Task FavoriteTrackAsync(int partyGoerId, string trackUri)
        {
            string sql = @"INSERT INTO FavoriteTracks(user_id, time, track_id)
                           VALUES(@UserId, NOW(), @TrackId)";

            await _connection.ExecuteAsync(sql, new
            {
                UserId = partyGoerId,
                TrackId = trackUri.Split("+").First()
            });
        }

        public async Task<List<string>> GetFavoriteTracksAsync(int partyGoerId)
        {
            string sql = @"SELECT track_id
                           FROM FavoriteTracks
                           WHERE user_id = @UserId";

            return (List<string>)await _connection.QueryAsync<string>(sql, new
            {
                UserId = partyGoerId
            });
        }

        public async Task<int?> SaveUserAsync(PartyGoer partier)
        {
            string newUserSql = @"INSERT INTO Users(name, display_name)
                                  VALUES(@Name, @Name)";

            string lastLoginSql = @"INSERT INTO LastLogin(user_id, time)
                                    VALUES(@UserId, NOW())";


            await _connection.ExecuteAsync(newUserSql, new
            {
                Name = partier.GetSpotifyId()
            });

            int? userId = await GetUserIdAsync(partier);

            if (!userId.HasValue)
            {
                throw new Exception($"User {partier.GetSpotifyId()} does not have an id after saving");
            }

            await _connection.ExecuteAsync(lastLoginSql, new
            {
                UserId = userId.Value
            });

            return await GetUserIdAsync(partier);
        }

        public async Task<int?> GetUserIdAsync(PartyGoer user)
        {
            string sql = @"SELECT id 
                           FROM Users
                           WHERE name = @Name";

            int? userId = await _connection.ExecuteScalarAsync<int?>(sql, new { Name = user.GetSpotifyId() });

            return userId;
        }

        public async Task UnfavoriteTrackAsync(int userId, string trackUri)
        {
            string sql = @"DELETE FROM FavoriteTracks 
                           WHERE track_id = @TrackId 
                           AND user_id = @UserId";

            await _connection.ExecuteAsync(sql, new
            {
                TrackId = trackUri.Split("+").First(),
                UserId = userId
            });
        }

        public async Task UpdateLastLoginTimeAsync(int userId)
        {
            string sql = @"UPDATE LastLogin
                           SET time = NOW()
                           where user_id = @UserId";

            await _connection.ExecuteAsync(sql, new { UserId = userId });
        }
    }
}
