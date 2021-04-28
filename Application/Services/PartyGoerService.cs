using Microsoft.AspNetCore.Http;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.Errors;
using System.Collections.Concurrent;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;
using SpotSync.Domain.Contracts.Repositories;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Application.Services
{
    public class PartyGoerService : IPartyGoerService
    {
        private ISpotifyHttpClient _spotifyHttpClient;
        private IHttpContextAccessor _httpContextAccessor;
        private ISpotifyAuthentication _spotifyAuthentication;
        private IDictionary<string, PartyGoer> _partyGoerCache;
        private ILogService _logService;
        private ISpotifyApi _spotifyApi;
        private IUserRepository _userRepository;
        private IDictionary<string, int> _userIds;
        private object _randomLock;
        private Random _random;

        public PartyGoerService(ISpotifyHttpClient spotifyHttpClient, IHttpContextAccessor httpContextAccessor,
        ISpotifyAuthentication spotifyAuthentication, ILogService logService, ISpotifyApi spotifyApi, IUserRepository userRepository)
        {
            _spotifyHttpClient = spotifyHttpClient;
            _httpContextAccessor = httpContextAccessor;
            _spotifyAuthentication = spotifyAuthentication;
            _partyGoerCache = new ConcurrentDictionary<string, PartyGoer>();
            _logService = logService;
            _spotifyApi = spotifyApi;
            _userRepository = userRepository;
            _userIds = new Dictionary<string, int>();
            _randomLock = new object();
            _random = new Random();
        }

        public async Task<List<SuggestedContribution>> GetSuggestedContributionsAsync(PartyGoer partier)
        {
            // get items from multiple sources and return that to the client
            //Task<List<SpotibroModels.PlaylistSummary>> playlistsTask = GetUsersPlaylistsAsync(partier, 10);
            Task<List<SpotibroModels.Track>> tracksTask = _spotifyHttpClient.GetUserTopTracksAsync(partier.GetSpotifyId());
            Task<List<SpotibroModels.Artist>> artistsTask = _spotifyApi.GetUsersTopArtistsAsync(partier);

            await Task.WhenAll(artistsTask, tracksTask);

            List<SuggestedContribution> contributions = new List<SuggestedContribution>();

            lock (_randomLock)
            {
                for (int i = 0; i < 2; i++)
                {
                    SpotibroModels.Track track = tracksTask.Result.ElementAt(_random.Next(0, tracksTask.Result.Count - 1));

                    contributions.Add(new SuggestedContribution
                    {
                        Id = track.Id,
                        Name = $"{track.Name} - {track.Artists.First().Name}",
                        Type = ContributionType.Track
                    });
                }
                /*
                for (int i = 0; i < 2; i++)
                {
                    SpotibroModels.PlaylistSummary playlist = playlistsTask.Result.ElementAt(_random.Next(0, playlistsTask.Result.Count - 1));

                    contributions.Add(new SuggestedContribution
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Type = ContributionType.Playlist
                    });
                }*/

                for (int i = 0; i < 2; i++)
                {
                    SpotibroModels.Artist artist = artistsTask.Result.ElementAt(_random.Next(0, artistsTask.Result.Count - 1));

                    contributions.Add(new SuggestedContribution
                    {
                        Id = artist.Id,
                        Name = artist.Name,
                        Type = ContributionType.Artist
                    });
                }
            }

            return contributions;
        }

        public async Task FavoriteTrackAsync(PartyGoer user, string trackUri)
        {
            await _userRepository.FavoriteTrackAsync(await GetUserIdAsync(user), trackUri);
        }

        public async Task UnfavoriteTrackAsync(PartyGoer user, string trackUri)
        {
            await _userRepository.UnfavoriteTrackAsync(await GetUserIdAsync(user), trackUri);
        }

        public async Task<int> LoginUser(PartyGoer user)
        {
            int? userId = await _userRepository.GetUserIdAsync(user);

            if (!userId.HasValue)
            {
                userId = await _userRepository.SaveUserAsync(user);

                if (userId.HasValue)
                {
                    if (!_userIds.ContainsKey(user.GetSpotifyId()))
                    {
                        _userIds.Add(user.GetSpotifyId(), userId.Value);
                    }
                }
                else
                {
                    throw new Exception($"User {user.GetSpotifyId()} does not have a user id even after saving user");
                }
            }

            await _userRepository.UpdateLastLoginTimeAsync(userId.Value);

            return userId.Value;
        }

        private async Task<int> GetUserIdAsync(PartyGoer user)
        {
            if (_userIds.ContainsKey(user.GetSpotifyId()))
            {
                return _userIds[user.GetSpotifyId()];
            }

            int? userId = await _userRepository.GetUserIdAsync(user);

            if (!userId.HasValue)
            {
                throw new Exception($"The user {user.GetSpotifyId()} does not have a user id. They need to be registered");
            }

            return userId.Value;
        }

        public async Task<CurrentSongDTO> GetCurrentSongAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetCurrentSongAsync(partyGoerId);
        }

        public async Task<List<SpotibroModels.Track>> GetRecommendedSongsAsync(string partyGoerId, int count = 10)
        {
            return await _spotifyHttpClient.GetUserTopTracksAsync(partyGoerId, count);
        }

        public async Task<string> GetUsersActiveDeviceAsync(string partyGoerId)
        {
            return await _spotifyHttpClient.GetUsersActiveDeviceAsync(partyGoerId);
        }

        public async Task<IEnumerable<ISpotifyQueryResult>> SearchSpotifyAsync(string query, SpotifyQueryType queryType, int limit = 10)
        {
            return await _spotifyHttpClient.QuerySpotifyAsync(await GetCurrentPartyGoerAsync(), query, queryType, limit);
        }

        /// <summary>
        /// Accesses the current party goer. If no party goer is associated with the current session, null is returned
        /// </summary>
        /// <returns>A party goer, if unauthenticated, null</returns>
        public async Task<PartyGoer> GetCurrentPartyGoerAsync()
        {
            string partyGoerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrWhiteSpace(partyGoerId)) return null;

            if (_partyGoerCache.ContainsKey(partyGoerId))
            {
                return _partyGoerCache[partyGoerId];
            }
            else
            {
                User user = await _spotifyHttpClient.GetUserDetailsAsync(partyGoerId);

                PartyGoer newPartyGoer = new PartyGoer(user.SpotifyId, user.ExplicitSettings.Filter, user.Market, user.Product);
                _partyGoerCache.TryAdd(partyGoerId, newPartyGoer);

                return newPartyGoer;
            }
        }


        public async Task<string> GetPartyGoerAccessTokenAsync(PartyGoer partyGoer)
        {
            if (await _spotifyAuthentication.DoesAccessTokenNeedRefreshAsync(partyGoer.GetId()))
            {
                await _spotifyHttpClient.RefreshTokenForUserAsync(partyGoer.GetId());
            }

            return await _spotifyAuthentication.GetAccessTokenAsync(partyGoer);
        }

        public async Task<SpotSync.Domain.ServiceResult<List<Device>>> GetUserDevicesAsync(PartyGoer partyGoer)
        {
            try
            {
                List<Device> devices = await _spotifyHttpClient.GetUserDevicesAsync(partyGoer);

                return new Domain.ServiceResult<List<Device>> { Result = devices };
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred while trying to get user devices");

                return new Domain.ServiceResult<List<Device>> { Error = ErrorType.SpotifyApiFailed };
            }
        }

        public async Task<List<SpotibroModels.PlaylistSummary>> GetUsersPlaylistsAsync(PartyGoer user, int limit = 10, int offset = 0)
        {
            return await _spotifyApi.GetUsersPlaylistsAsync(user, limit, offset);
        }

        public async Task<SpotibroModels.PlaylistContents> GetPlaylistItemsAsync(PartyGoer user, string playlistId)
        {
            return await _spotifyApi.GetPlaylistContentsAsync(user, playlistId);
        }

        public async Task<List<string>> GetUsersFavoriteTracksAsync(PartyGoer user)
        {
            return await _userRepository.GetFavoriteTracksAsync(await GetUserIdAsync(user));
        }
    }
}
